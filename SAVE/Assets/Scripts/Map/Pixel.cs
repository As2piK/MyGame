using System;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour, INode {

    public static int NextID;

    public Color hoverColor;
    public Color busyHoverColor;

    public int x;
    public int y;
    public int id;

    private Renderer rend;
    public Color defaultColor;
    public Color currentColor;
    public GameObject turret;
    public Vector3 positionOffset;

    [HideInInspector]
    public bool canBuild = true;

    #region A*
    private bool isOpenList = false;
    private bool isClosedList = false;
    public bool IsWall { get { return turret != null; } }
    public int TotalCost { get { return MovementCost + EstimatedCost; } }
    public int MovementCost { get; private set; }
    public int EstimatedCost { get; private set; }
    public INode Parent { get; set; }
    #endregion

    private void Start()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
        id = NextID;
        NextID++;
    }

    public Vector3 getBuildPosition()
    {
        return transform.position + positionOffset;
    }

    private void OnMouseDown()
    {
        //BuildTurret
        buildTurret();
    }

    public void buildTurret()
    {
        if (canBuild)
        {
            GameObject oldTurret = turret;
            if (turret == null)
            {
                turret = Instantiate(GameManager.instance.getTurretToBuild(), getBuildPosition(), transform.rotation);
                if (!GameManager.instance.map.addTurretTo(turret, this))
                {
                    Destroy(turret);
                    turret = oldTurret;
                    
                }
            }
            else
            {
                Destroy(turret);
                turret = null;
                GameManager.instance.map.addTurretTo(turret, this);
            }
        }
    }

    private void OnMouseEnter()
    {
        currentColor = rend.material.color;
        if (canBuild)
        {
            rend.material.color = hoverColor;
        } else
        {
            rend.material.color = busyHoverColor;
        }
    }

    private void OnMouseExit()
    {
        rend.material.color = currentColor;
    }

    #region A* Interface


    public bool IsOpenList(IEnumerable<INode> openList)
    {
        return isOpenList;
    }

    public void SetOpenList(bool value)
    {
        isOpenList = value;
    }

    public bool IsClosedList(IEnumerable<INode> closedList)
    {
        return IsWall || isClosedList;
    }

    public void SetClosedList(bool value)
    {
        isClosedList = value;
    }

    public void SetMovementCost(INode parent)
    {
        MovementCost = parent.MovementCost + 1;
    }

    public void SetEstimatedCost(INode goal)
    {
        var g = (Pixel)goal;
        this.EstimatedCost = Math.Abs(this.x - g.x) + Math.Abs(this.y - g.y);
    }

    public bool IsGoal(INode goal)
    {
        return this.x == ((Pixel)goal).x && this.y == ((Pixel)goal).y;
    }

    //private static int[] childXPos = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
    //private static int[] childYPos = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };
    private static int[] childXPos = new int[] { -1, 0, 0, 1 };
    private static int[] childYPos = new int[] { 0, -1, 1, 0 };

    private IEnumerable<INode> Childrens;
    public IEnumerable<INode> Children
    {
        get
        {
            if (Childrens == null) {
                var children = new List<INode>();

                for (int i = 0; i < childXPos.Length; i++)
                {
                    // skip any nodes out of bounds.
                    if (x + childXPos[i] >= GameManager.instance.map.mapSizeX || y + childYPos[i] >= GameManager.instance.map.mapSizeY)
                        continue;
                    if (x + childXPos[i] < 0 || y + childYPos[i] < 0)
                        continue;

                    children.Add(GameManager.instance.map.map[x + childXPos[i], y + childYPos[i]]);
                }

                Childrens = children;
            }
            return Childrens;
        }
    }
    #endregion
}
