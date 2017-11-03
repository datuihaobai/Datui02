using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour 
{
    public GameObject root;
    public Transform boxBoundLB;//云遮罩的范围，左下leftbottom
    public Transform boxBoundRT;//云遮罩的范围，右上righttop
    public GameObject normal;
    public List<GameObject> edges;

    public int indexX;
    public int indexY;

    public void CheckShow(PATileTerrain tileTerrain)
    {
        Vector3 boundLBPos = boxBoundLB.position;
        Vector3 boundLTPos = new Vector3(boxBoundLB.position.x, boxBoundLB.position.y, boxBoundRT.position.z);
        Vector3 boundRTPos = boxBoundRT.position;
        Vector3 boundRBPos = new Vector3(boxBoundRT.position.x, boxBoundLB.position.y, boxBoundLB.position.z);

        PATileTerrain.PATile hitTileLB = tileTerrain.GetTileByRay(boundLBPos, Vector3.down);
        PATileTerrain.PATile hitTileLT = tileTerrain.GetTileByRay(boundLTPos, Vector3.down);
        PATileTerrain.PATile hitTileRT = tileTerrain.GetTileByRay(boundRTPos, Vector3.down);
        PATileTerrain.PATile hitTileRB = tileTerrain.GetTileByRay(boundRBPos, Vector3.down);

        if(PATileTerrain.PATile.IsNotBaseElement(hitTileLB) ||
            PATileTerrain.PATile.IsNotBaseElement(hitTileLT) ||
            PATileTerrain.PATile.IsNotBaseElement(hitTileRT) ||
            PATileTerrain.PATile.IsNotBaseElement(hitTileRB))
            Hide();
        else
            Show();

    }

    public void CheckEdge(PATileTerrain tileTerrain)
    {
        if (!root.activeInHierarchy)
            return;
        Cloud[] neighborClouds = tileTerrain.GetNeighboringCloudsNxN(indexX, indexY, 1);

        for (int i = 0; i < neighborClouds.Length ; i++ )
        {
            Cloud neighborCloud = neighborClouds[i];
            if (neighborCloud == null)
                continue;
            if(!neighborCloud.root.activeInHierarchy)
            {
                int randomIndex = RandomManager.instance.Range(0,edges.Count);
                edges[randomIndex].SetActive(true);
                normal.SetActive(false);
                return;
            }
        }
    }

    void Show()
    {
        root.SetActive(true);
        normal.SetActive(true);
        foreach(var edge in edges)
            edge.SetActive(false);
    }

    void Hide()
    {
        root.SetActive(false);
    }
}