using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AStar algorithm states while searching for the goal.
/// </summary>
public enum State
{
    /// <summary>
    /// The AStar algorithm is still searching for the goal.
    /// </summary>
    Searching,

    /// <summary>
    /// The AStar algorithm has found the goal.
    /// </summary>
    GoalFound,

    /// <summary>
    /// The AStar algorithm has failed to find a solution.
    /// </summary>
    Failed
}

/// <summary>
/// System.Collections.Generic.SortedList by default does not allow duplicate items.
/// Since items are keyed by TotalCost there can be duplicate entries per key.
/// </summary>
internal class DuplicateComparer : IComparer<int>
{
    public int Compare(int x, int y)
    {
        return (x <= y) ? -1 : 1;
    }
}

/// <summary>
/// Interface to setup and run the AStar algorithm.
/// </summary>
public class AStar
{

    private static AStar instance;
    
    /// <summary>
    /// The open list.
    /// </summary>
    private SortedList<int, INode> openList;

    /// <summary>
    /// The closed list.
    /// </summary>
    private SortedList<int, INode> closedList;

    /// <summary>
    /// The current node.
    /// </summary>
    private INode current;

    /// <summary>
    /// The goal node.
    /// </summary>
    private INode goal;

    /// <summary>
    /// Gets the current amount of steps that the algorithm has performed.
    /// </summary>
    public int Steps { get; private set; }

    /// <summary>
    /// Gets the current state of the open list.
    /// </summary>
    public IEnumerable<INode> OpenList { get { return openList.Values; } }

    /// <summary>
    /// Gets the current state of the closed list.
    /// </summary>
    public IEnumerable<INode> ClosedList { get { return closedList.Values; } }

    /// <summary>
    /// Gets the current node that the AStar algorithm is at.
    /// </summary>
    public INode CurrentNode { get { return current; } }

    public static AStar getInstance(INode start, INode goal)
    {
        if (instance == null)
        {
            instance = new AStar();
        }
        instance.Reset(start, goal);
        return instance;
    }

    public static AStar getInstance()
    {
        if (instance == null)
        {
            instance = new AStar();
        }
        return instance;
    }

    /// <summary>
    /// Creates a new AStar algorithm instance with the provided start and goal nodes.
    /// </summary>
    /// <param name="start">The starting node for the AStar algorithm.</param>
    /// <param name="goal">The goal node for the AStar algorithm.</param>
    private AStar()
    {
        var duplicateComparer = new DuplicateComparer();
        openList = new SortedList<int, INode>(duplicateComparer);
        closedList = new SortedList<int, INode>(duplicateComparer);
    }

    /// <summary>
    /// Resets the AStar algorithm with the newly specified start node and goal node.
    /// </summary>
    /// <param name="start">The starting node for the AStar algorithm.</param>
    /// <param name="goal">The goal node for the AStar algorithm.</param>
    public void Reset(INode start, INode g)
    {
        Steps = 0;

        //TEMP

        for (int i = 0; i < BuildManager.instance.map.mapSizeX; i++)
        {
            for (int j = 0; j < BuildManager.instance.map.mapSizeY; j++)
            {
                //BuildManager.instance.map.map[i, j].Parent = null;
            }
        }

        foreach (INode n in openList.Values)
        {
            n.SetOpenList(false);
        }
        openList.Clear();
        foreach (INode n in closedList.Values)
        {
            n.SetClosedList(false);
        }
        
        closedList.Clear();
        current = start;
        goal = g;
        current.SetOpenList(true);
        openList.Add(current);
    }

    /// <summary>
    /// Steps the AStar algorithm forward until it either fails or finds the goal node.
    /// </summary>
    /// <returns>Returns the state the algorithm finished in, Failed or GoalFound.</returns>
    public State Run()
    {
        // Continue searching until either failure or the goal node has been found.
        while (true)
        {
            State s = Step();
            if (s != State.Searching)
                return s;
        }
    }

    /// <summary>
    /// Moves the AStar algorithm forward one step.
    /// </summary>
    /// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
    public State Step()
    {
        Steps++;
        while (true)
        {
            // There are no more nodes to search, return failure.
            if (openList.IsEmpty())
            {
                Debug.LogError("Path not found");
                return State.Failed;
            }

            // Check the next best node in the graph by TotalCost.
            current = openList.Pop();

            // This node has already been searched, check the next one.
            if (current.IsClosedList())
            {
                continue;
            }

            // An unsearched node has been found, search it.
            break;
        }

        // Remove from the open list and place on the closed list 
        // since this node is now being searched.
        current.SetOpenList(false);
        closedList.Add(current);
        current.SetClosedList(true);

        // Found the goal, stop searching.
        if (current.IsGoal(goal))
        {
            return State.GoalFound;
        }

        // Node was not the goal so add all children nodes to the open list.
        // Each child needs to have its movement cost set and estimated cost.
        foreach (INode child in current.Children)
        {
            // If the child has already been searched (closed list) or is on
            // the open list to be searched then do not modify its movement cost
            // or estimated cost since they have already been set previously.
            if (child.IsOpenList() || child.IsClosedList())
            {
                continue;
            }

            child.Parent = current;
            child.SetMovementCost(current);
            child.SetEstimatedCost(goal);
            openList.Add(child);
            child.SetOpenList(true);
        }

        // This step did not find the goal so return status of still searching.
        return State.Searching;
    }

    /// <summary>
    /// Gets the path of the last solution of the AStar algorithm.
    /// Will return a partial path if the algorithm has not finished yet.
    /// </summary>
    /// <returns>Returns null if the algorithm has never been run.</returns>
    public List<INode> GetPath()
    {
        if (current != null)
        {
            var next = current;
            var path = new List<INode>();
            while (next != null)
            {
                path.Add(next);
                next = next.Parent;
            }
            path.Reverse();
            return path;
        }
        return null;
    }
}
