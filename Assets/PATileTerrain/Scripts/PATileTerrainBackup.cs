using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PATileTerrain
{
    public void PaintTile5x5(PATile tile, int t)
    {
        if (tile == null) return;
        int i;
        t = Mathf.Clamp(t, 0, settings.tsTypes.Count);

        IntermediateInfo[] imInfo = new IntermediateInfo[24];
        PATile[] tile2 = GetNeighboringTilesNxN(tile, 1);
        PATile[] tile3 = GetNeighboringTilesNxN(tile, 3);
        PATile[] nTiles = GetNeighboringTilesNxN(tile, 5);
        PATile[] tls = GetNeighboringTilesNxN(tile, 7);

        tile.SetTileProp(t,t,0);
        UpdateTileUV(tile);

        for (i = 0; i < 8; ++i)
            if (tile2[i] != null)
            {
                tile2[i].SetTileProp(t,t,0);
                UpdateTileUV(tile2[i]);
            }
        for (i = 0; i < 16; ++i)
            if (tile3[i] != null)
            {
                tile3[i].SetTileProp(t,t,0);
                UpdateTileUV(tile3[i]);
            }

        //Array visualization, where 'c' = current tile
        // 0  1  2  3  4  5  6  7  8
        // 31 0  1  2  3  4  5  6  9
        // 30 23 x  x  x  x  x  7  10
        // 29 22 x  x  x  x  x  8  11
        // 28 21 x  x  c  x  x  9  12
        // 27 20 x  x  x  x  x  10 13
        // 26 19 x  x  x  x  x  11 14
        // 25 18 17 16 15 14 13 12 15
        // 24 23 22 21 20 19 18 17 16
        //0	
        CalcTileBits(t, nTiles[0], 2, out imInfo[0]);
        //1
        CalcTileBits(t, nTiles[1], 3, out imInfo[1]);
        //2
        CalcTileBits(t, nTiles[2], 3, out imInfo[2]);
        //3
        CalcTileBits(t, nTiles[3], 3, out imInfo[3]);
        //4
        CalcTileBits(t, nTiles[4], 3, out imInfo[4]);
        //5
        CalcTileBits(t, nTiles[5], 3, out imInfo[5]);
        //6
        CalcTileBits(t, nTiles[6], 1, out imInfo[6]);

        //7
        CalcTileBits(t, nTiles[7], 9, out imInfo[7]);
        //8
        CalcTileBits(t, nTiles[8], 9, out imInfo[8]);
        //9
        CalcTileBits(t, nTiles[9], 9, out imInfo[9]);
        //10
        CalcTileBits(t, nTiles[10], 9, out imInfo[10]);
        //11
        CalcTileBits(t, nTiles[11], 9, out imInfo[11]);

        //12	
        CalcTileBits(t, nTiles[12], 8, out imInfo[12]);
        //13
        CalcTileBits(t, nTiles[13], 12, out imInfo[13]);
        //14
        CalcTileBits(t, nTiles[14], 12, out imInfo[14]);
        //15
        CalcTileBits(t, nTiles[15], 12, out imInfo[15]);
        //16
        CalcTileBits(t, nTiles[16], 12, out imInfo[16]);
        //17
        CalcTileBits(t, nTiles[17], 12, out imInfo[17]);
        //18
        CalcTileBits(t, nTiles[18], 4, out imInfo[18]);

        //19
        CalcTileBits(t, nTiles[19], 6, out imInfo[19]);
        //20
        CalcTileBits(t, nTiles[20], 6, out imInfo[20]);
        //21
        CalcTileBits(t, nTiles[21], 6, out imInfo[21]);
        //22
        CalcTileBits(t, nTiles[22], 6, out imInfo[22]);
        //23
        CalcTileBits(t, nTiles[23], 6, out imInfo[23]);

        //0	
        //CalcIntermediateTileBits(tls[0], 2, imInfo[0]);
        ////1
        //CalcIntermediateTileBits(tls[1], 3, imInfo[0]);
        ////2
        //CalcIntermediateTileBits(tls[2], 3, imInfo[1]);
        ////3
        //CalcIntermediateTileBits(tls[3], 3, imInfo[2]);
        ////4
        //CalcIntermediateTileBits(tls[4], 3, imInfo[3]);
        ////5
        //CalcIntermediateTileBits(tls[5], 3, imInfo[4]);
        ////6
        //CalcIntermediateTileBits(tls[6], 1, imInfo[4]);

        ////7
        //CalcIntermediateTileBits(tls[7], 9, imInfo[4]);
        ////8
        //CalcIntermediateTileBits(tls[8], 9, imInfo[5]);
        ////9
        //CalcIntermediateTileBits(tls[9], 9, imInfo[6]);	
        ////10
        //CalcIntermediateTileBits(tls[10], 9, imInfo[7]);
        ////11
        //CalcIntermediateTileBits(tls[11], 9, imInfo[8]);

        ////12	
        //CalcIntermediateTileBits(tls[12], 8, imInfo[8]);
        ////13
        //CalcIntermediateTileBits(tls[13], 12, imInfo[8]);		
        ////14
        //CalcIntermediateTileBits(tls[14], 12, imInfo[9]);
        ////15
        //CalcIntermediateTileBits(tls[15], 12, imInfo[10]);
        ////16
        //CalcIntermediateTileBits(tls[16], 12, imInfo[11]);
        ////17
        //CalcIntermediateTileBits(tls[17], 12, imInfo[12]);
        ////18
        //CalcIntermediateTileBits(tls[18], 4, imInfo[12]);

        ////19
        //CalcIntermediateTileBits(tls[19], 6, imInfo[12]);
        ////20
        //CalcIntermediateTileBits(tls[20], 6, imInfo[13]);
        ////21
        //CalcIntermediateTileBits(tls[21], 6, imInfo[14]);
        ////22
        //CalcIntermediateTileBits(tls[22], 6, imInfo[15]);			
        ////23
        //CalcIntermediateTileBits(tls[23], 6, imInfo[0]);
    }

    public void PaintTile3x3(PATile tile, int t)
    {
        if (tile == null) return;
        int i;
        t = Mathf.Clamp(t, 0, settings.tsTypes.Count);

        IntermediateInfo[] imInfo = new IntermediateInfo[16];
        PATile[] tile2 = GetNeighboringTilesNxN(tile, 1);
        PATile[] nTiles = GetNeighboringTilesNxN(tile, 3);
        PATile[] tls = GetNeighboringTilesNxN(tile, 5);

        tile.SetTileProp(t,t,0);
        UpdateTileUV(tile);

        for (i = 0; i < 8; ++i)
            if (tile2[i] != null)
            {
                tile2[i].SetTileProp(t,t,0);
                UpdateTileUV(tile2[i]);
            }

        //Array visualization, where 'c' = current tile
        // 0  1  2  3  4  5  6
        // 23 0  1  2  3  4  7
        // 22 15 x  x  x  5  8
        // 21 14 x  c  x  6  9
        // 20 13 x  x  x  7  10
        // 19 12 11 10 9  8  11
        // 18 17 16 15 14 13 12

        //0	
        CalcTileBits(t, nTiles[0], 2, out imInfo[0]);
        //1
        CalcTileBits(t, nTiles[1], 3, out imInfo[1]);
        //2
        CalcTileBits(t, nTiles[2], 3, out imInfo[2]);
        //3
        CalcTileBits(t, nTiles[3], 3, out imInfo[3]);
        //4
        CalcTileBits(t, nTiles[4], 1, out imInfo[4]);

        //5
        CalcTileBits(t, nTiles[5], 9, out imInfo[5]);
        //6
        CalcTileBits(t, nTiles[6], 9, out imInfo[6]);
        //7
        CalcTileBits(t, nTiles[7], 9, out imInfo[7]);

        //8	
        CalcTileBits(t, nTiles[8], 8, out imInfo[8]);
        //9
        CalcTileBits(t, nTiles[9], 12, out imInfo[9]);
        //10
        CalcTileBits(t, nTiles[10], 12, out imInfo[10]);
        //11
        CalcTileBits(t, nTiles[11], 12, out imInfo[11]);
        //12
        CalcTileBits(t, nTiles[12], 4, out imInfo[12]);

        //13
        CalcTileBits(t, nTiles[13], 6, out imInfo[13]);
        //14
        CalcTileBits(t, nTiles[14], 6, out imInfo[14]);
        //15
        CalcTileBits(t, nTiles[15], 6, out imInfo[15]);

        //Array visualization, where 'c' = current tile
        // 0  1  2  3  4  5  6
        // 23 0  1  2  3  4  7
        // 22 15 x  x  x  5  8
        // 21 14 x  c  x  6  9
        // 20 13 x  x  x  7  10
        // 19 12 11 10 9  8  11
        // 18 17 16 15 14 13 12

        //0	
        //CalcIntermediateTileBits(tls[0], 2, imInfo[0]);
        ////1
        //CalcIntermediateTileBits(tls[1], 3, imInfo[0]);
        ////2
        //CalcIntermediateTileBits(tls[2], 3, imInfo[1]);
        ////3
        //CalcIntermediateTileBits(tls[3], 3, imInfo[2]);
        ////4
        //CalcIntermediateTileBits(tls[4], 3, imInfo[3]);
        ////5
        //CalcIntermediateTileBits(tls[5], 3, imInfo[4]);
        ////6
        //CalcIntermediateTileBits(tls[6], 1, imInfo[4]);

        ////7
        //CalcIntermediateTileBits(tls[7], 9, imInfo[4]);
        ////8
        //CalcIntermediateTileBits(tls[8], 9, imInfo[5]);
        ////9
        //CalcIntermediateTileBits(tls[9], 9, imInfo[6]);	
        ////10
        //CalcIntermediateTileBits(tls[10], 9, imInfo[7]);
        ////11
        //CalcIntermediateTileBits(tls[11], 9, imInfo[8]);

        ////12	
        //CalcIntermediateTileBits(tls[12], 8, imInfo[8]);
        ////13
        //CalcIntermediateTileBits(tls[13], 12, imInfo[8]);		
        ////14
        //CalcIntermediateTileBits(tls[14], 12, imInfo[9]);
        ////15
        //CalcIntermediateTileBits(tls[15], 12, imInfo[10]);
        ////16
        //CalcIntermediateTileBits(tls[16], 12, imInfo[11]);
        ////17
        //CalcIntermediateTileBits(tls[17], 12, imInfo[12]);
        ////18
        //CalcIntermediateTileBits(tls[18], 4, imInfo[12]);

        ////19
        //CalcIntermediateTileBits(tls[19], 6, imInfo[12]);
        ////20
        //CalcIntermediateTileBits(tls[20], 6, imInfo[13]);
        ////21
        //CalcIntermediateTileBits(tls[21], 6, imInfo[14]);
        ////22
        //CalcIntermediateTileBits(tls[22], 6, imInfo[15]);			
        ////23
        //CalcIntermediateTileBits(tls[23], 6, imInfo[0]);
    }

    public void PaintTile1x1(PATile tile, int t)
    {
        if (tile == null) return;
        t = Mathf.Clamp(t, 0, settings.tsTypes.Count);

        //int i, transitionId;
        //PATSTransition transition = FindTransitionForType(t, out transitionId);
        //if (transition == null) return;

        IntermediateInfo[] imInfo = new IntermediateInfo[8];
        PATile[] nTiles = GetNeighboringTilesNxN(tile, 1);
        PATile[] tls = GetNeighboringTilesNxN(tile, 3);

        //Current tile		
        tile.SetTileProp(t,t,0);
        UpdateTileUV(tile);

        //0	
        CalcTileBits(t, nTiles[0], 2, out imInfo[0]);
        //1
        CalcTileBits(t, nTiles[1], 3, out imInfo[1]);
        //2
        CalcTileBits(t, nTiles[2], 1, out imInfo[2]);
        //3
        CalcTileBits(t, nTiles[3], 9, out imInfo[3]);
        //4
        CalcTileBits(t, nTiles[4], 8, out imInfo[4]);
        //5
        CalcTileBits(t, nTiles[5], 12, out imInfo[5]);
        //6
        CalcTileBits(t, nTiles[6], 4, out imInfo[6]);
        //7
        CalcTileBits(t, nTiles[7], 6, out imInfo[7]);

        //Array visualization, where 'c' = current tile
        // 0  1  2  3  4
        //15  0  1  2  5
        //14  7  c  3  6
        //13  6  5  4  7 
        //12 11 10  9  8 	

        //0	
        CalcIntermediateTileBits(tls[0], 2, imInfo[0]);
        //1
        CalcIntermediateTileBits(tls[1], 3, imInfo[0]);
        //2
        CalcIntermediateTileBits(tls[2], 3, imInfo[1]);
        //3
        CalcIntermediateTileBits(tls[3], 3, imInfo[2]);
        //4
        CalcIntermediateTileBits(tls[4], 1, imInfo[2]);

        //5
        CalcIntermediateTileBits(tls[5], 9, imInfo[2]);
        //6
        CalcIntermediateTileBits(tls[6], 9, imInfo[3]);
        //7
        CalcIntermediateTileBits(tls[7], 9, imInfo[4]);

        //8	
        CalcIntermediateTileBits(tls[8], 8, imInfo[4]);
        //9
        CalcIntermediateTileBits(tls[9], 12, imInfo[4]);
        //10
        CalcIntermediateTileBits(tls[10], 12, imInfo[5]);
        //11
        CalcIntermediateTileBits(tls[11], 12, imInfo[6]);
        //12
        CalcIntermediateTileBits(tls[12], 4, imInfo[6]);

        //13
        CalcIntermediateTileBits(tls[13], 6, imInfo[6]);
        //14
        CalcIntermediateTileBits(tls[14], 6, imInfo[7]);
        //15
        CalcIntermediateTileBits(tls[15], 6, imInfo[0]);
    }

    public void SetVertexColors(int x, int y, bool rect, float power, Color clr, float radius, List<PAPointXY> points)
    {
        int i, j;
        int count = (int)(radius / settings.tileSize);
        int xMin = x, xMax = x, yMin = y, yMax = y;
        //float distance;
        Vector3 currentPos, tilePos;
        PAPointXY p;

        xMin = x - count; xMin = Mathf.Clamp(xMin, 0, settings.xCount);
        yMin = y - count; yMin = Mathf.Clamp(yMin, 0, settings.yCount);
        xMax = x + count; xMax = Mathf.Clamp(xMax, 0, settings.xCount);
        yMax = y + count; yMax = Mathf.Clamp(yMax, 0, settings.yCount);

        currentPos = new Vector3(x * settings.tileSize, 0.0f, y * settings.tileSize);

        for (j = yMin; j <= yMax; ++j)
            for (i = xMin; i <= xMax; ++i)
            {
                tilePos = new Vector3(i * settings.tileSize, 0.0f, j * settings.tileSize);

                if (!rect && Vector3.Distance(tilePos, currentPos) > radius) continue;
                p = new PAPointXY(i, j); if (!points.Contains(p)) points.Add(p);
                SetVertexColor(i, j, power, clr);
            }
    }

    protected void SetVertexColor(int x, int y, float power, Color clr)
    {
        PATile tile;
        int i, j, id;
        bool c;
        List<Mesh> ms = new List<Mesh>();
        List<Color[]> clrs = new List<Color[]>();

        Mesh mesh;
        Color[] colors;

        PAPoint point = settings.points[(settings.xCount + 1) * y + x];
        for (i = 0; i < 4; ++i)
            if (point.t[i] >= 0)
            {
                c = false;
                tile = GetTile(point.t[i]);

                mesh = GetChunkMesh(tile.chunkId);
                colors = null;

                for (j = 0; j < ms.Count; ++j) if (ms[j] == mesh) { c = true; colors = clrs[j]; }
                if (colors == null) colors = mesh.colors;

                id = tile.cId * 4 + point.p[i];
                colors[id] = Color.Lerp(colors[id], clr, power);

                if (!c) { ms.Add(mesh); clrs.Add(colors); }
            }
        for (i = 0; i < ms.Count; ++i) ms[i].colors = clrs[i];
    }


    /*public PATile[] GetNeighboringTiles1x1(PATile tile)
    {
        //Array visualization, where 'x' = current tile
        // 0  1  2
        // 7  x  3
        // 6  5  4
        PATile[] t = new PATile[8]; 
        for (int i = 0; i < 8; ++i) t[i] = null;

        if (tile.x > 0) //0, 1, 2
        {
            //0
            if (tile.y > 0) t[0] = GetTile(tile.x - 1, tile.y - 1);
            //1
            t[1] = GetTile(tile.x - 1, tile.y);
            //2
            if (tile.y < settings.yCount - 1) t[2] = GetTile(tile.x - 1, tile.y + 1);
        }
		
        if (tile.x < settings.xCount - 1) //4, 5, 6
        {
            //4
            if (tile.y < settings.yCount - 1) t[4] = GetTile(tile.x + 1, tile.y + 1);	
            //5
            t[5] = GetTile(tile.x + 1, tile.y);
            //6
            if (tile.y > 0) t[6] = GetTile(tile.x + 1, tile.y - 1);	
        }
		
        //7
        if (tile.y > 0) t[7] = GetTile(tile.x, tile.y - 1);
        //3
        if (tile.y < settings.yCount - 1) t[3] = GetTile(tile.x, tile.y + 1);
		
        for (int i = 0; i < 8; ++i) CheckTile(t[i]);
        return t;
    }
	
    public PATile[] GetNeighboringTiles3x3(PATile tile)
    {
        //Array visualization, where 'c' = current tile
        // 0  1  2  3  4
        //15  x  x  x  5
        //14  x  c  x  6
        //13  x  x  x  7 
        //12 11 10  9  8 
        PATile[] t = new PATile[16]; 
        for (int i = 0; i < 16; ++i) t[i] = null;
		
        if (tile.x > 1) //0, 1, 2, 3, 4
        {
            //0
            if (tile.y > 1) t[0] = GetTile(tile.x - 2, tile.y - 2);
            //1
            if (tile.y > 0) t[1] = GetTile(tile.x - 2, tile.y - 1);
            //2
            t[2] = GetTile(tile.x - 2, tile.y);
            //3
            if (tile.y < settings.yCount - 1) t[3] = GetTile(tile.x - 2, tile.y + 1);
            //4
            if (tile.y < settings.yCount - 2) t[4] = GetTile(tile.x - 2, tile.y + 2);				
        }
		
        if (tile.x < settings.xCount - 2) //12, 11, 10, 9, 8
        {
            //8
            if (tile.y < settings.yCount - 2) t[8] = GetTile(tile.x + 2, tile.y + 2);
            //9
            if (tile.y < settings.yCount - 1) t[9] = GetTile(tile.x + 2, tile.y + 1);
            //10
            t[10] = GetTile(tile.x + 2, tile.y);
            //11
            if (tile.y > 0) t[11] = GetTile(tile.x + 2, tile.y - 1);
            //12
            if (tile.y > 1) t[12] = GetTile(tile.x + 2, tile.y - 2);
        }
		
        //5, 6, 7
        if (tile.y < settings.yCount - 2)
        {
            //5
            if (tile.x > 0) t[5] = GetTile(tile.x - 1, tile.y + 2);
            //6
            t[6] = GetTile(tile.x, tile.y + 2);
            //7
            if (tile.x < settings.xCount - 1) t[7] = GetTile(tile.x + 1, tile.y + 2);
        }	
		
        //13, 14, 15
        if (tile.y > 1)
        {
            //13
            if (tile.x < settings.xCount - 1) t[13] = GetTile(tile.x + 1, tile.y - 2);
            //14
            t[14] = GetTile(tile.x, tile.y - 2);
            //15
            if (tile.x > 0) t[15] = GetTile(tile.x - 1, tile.y - 2);
        }	
		
        return t;
    }
	
    public PATile[] GetNeighboringTiles5x5(PATile tile)
    {
        //Array visualization, where 'c' = current tile
        // 0  1  2  3  4  5  6
        // 23 x  x  x  x  x  7
        // 22 x  x  x  x  x  8
        // 21 x  x  c  x  x  9
        // 20 x  x  x  x  x  10
        // 19 x  x  x  x  x  11
        // 18 17 16 15 14 13 12
        PATile[] t = new PATile[24]; 
        for (int i = 0; i < 24; ++i) t[i] = null;
		
        if (tile.x > 2) //0, 1, 2, 3, 4, 5, 6
        {
            //0
            if (tile.y > 2) t[0] = GetTile(tile.x - 3, tile.y - 3);
            //1
            if (tile.y > 1) t[1] = GetTile(tile.x - 3, tile.y - 2);
            //2
            if (tile.y > 0) t[2] = GetTile(tile.x - 3, tile.y - 1);
            //3
            t[3] = GetTile(tile.x - 3, tile.y);
            //4
            if (tile.y < settings.yCount - 1) t[4] = GetTile(tile.x - 3, tile.y + 1);
            //5
            if (tile.y < settings.yCount - 2) t[5] = GetTile(tile.x - 3, tile.y + 2);
            //6
            if (tile.y < settings.yCount - 3) t[6] = GetTile(tile.x - 3, tile.y + 3);				
        }
		
        if (tile.x < settings.xCount - 3) //12, 13, 14, 15, 16, 17, 18
        {
            //12
            if (tile.y < settings.yCount - 3) t[12] = GetTile(tile.x + 3, tile.y + 3);
            //13
            if (tile.y < settings.yCount - 2) t[13] = GetTile(tile.x + 3, tile.y + 2);
            //14
            if (tile.y < settings.yCount - 1) t[14] = GetTile(tile.x + 3, tile.y + 1);
            //15
            t[15] = GetTile(tile.x + 3, tile.y);
            //16
            if (tile.y > 0) t[16] = GetTile(tile.x + 3, tile.y - 1);
            //17
            if (tile.y > 1) t[17] = GetTile(tile.x + 3, tile.y - 2);
            //18
            if (tile.y > 2) t[18] = GetTile(tile.x + 3, tile.y - 3);
        }
		
        //Array visualization, where 'c' = current tile
        // 0  1  2  3  4  5  6
        // 23 x  x  x  x  x  7
        // 22 x  x  x  x  x  8
        // 21 x  x  c  x  x  9
        // 20 x  x  x  x  x  10
        // 19 x  x  x  x  x  11
        // 18 17 16 15 14 13 12
		
        //7, 8, 9, 10, 11
        if (tile.y < settings.yCount - 3)
        {
            //7
            if (tile.x > 1) t[7] = GetTile(tile.x - 2, tile.y + 3);
            //8
            if (tile.x > 0) t[8] = GetTile(tile.x - 1, tile.y + 3);
            //9
            t[9] = GetTile(tile.x, tile.y + 3);
            //10
            if (tile.x < settings.xCount - 1) t[10] = GetTile(tile.x + 1, tile.y + 3);
            //11
            if (tile.x < settings.xCount - 2) t[11] = GetTile(tile.x + 2, tile.y + 3);
        }	
		
        //19, 20, 21, 22, 23
        if (tile.y > 2)
        {
            //19
            if (tile.x < settings.xCount - 2) t[19] = GetTile(tile.x + 2, tile.y - 3);
            //20
            if (tile.x < settings.xCount - 1) t[20] = GetTile(tile.x + 1, tile.y - 3);
            //21
            t[21] = GetTile(tile.x, tile.y - 3);
            //22
            if (tile.x > 0) t[22] = GetTile(tile.x - 1, tile.y - 3);
            //23
            if (tile.x > 1) t[23] = GetTile(tile.x - 2, tile.y - 3);
        }	
		
        return t;
    }*/

    public bool FindPath(int startX, int startY, int targetX, int targetY, out PathData path)
    {
        //float startTime = Time.realtimeSinceStartup;

        int onOpenList = 0,
            parentXval = 0,
            parentYval = 0,
            a = 0, b = 0, m = 0, u = 0, v = 0,
            temp = 0, numberOfOpenListItems = 0,
            addedGCost = 0, tempGcost = 0,
            tempx, pathX, pathY,
            newOpenListItemID = 0,
            mapWidth = settings.xCount,
            mapHeight = settings.yCount;
        bool corner = false;
        bool found = false;
        int[] openList = new int[settings.xCount * settings.yCount + 2];
        int[,] whichList = new int[settings.xCount + 1, settings.yCount + 1];
        int[] openX = new int[settings.xCount * settings.yCount + 2];
        int[] openY = new int[settings.xCount * settings.yCount + 2];
        int[,] parentX = new int[settings.xCount + 1, settings.yCount + 1];
        int[,] parentY = new int[settings.xCount + 1, settings.yCount + 1];
        int[] Fcost = new int[settings.xCount * settings.yCount + 2];
        int[,] Gcost = new int[settings.xCount + 1, settings.yCount + 1];
        int[] Hcost = new int[settings.xCount * settings.yCount + 2];
        int pathLength = 0;
        int pathLocation = 0;
        int onClosedList = 10;

        path = new PathData();
        path.data = null;
        path.length = 0;
        path.found = false;

        if (startX >= mapWidth || startX < 0) return false;
        else if (startY >= mapHeight || startY < 0) return false;
        else if (targetX >= mapWidth || targetX < 0) return false;
        else if (targetY >= mapHeight || targetY < 0) return false;

        if (startX == targetX && startY == targetY && pathLocation > 0) return false;
        if (startX == targetX && startY == targetY && pathLocation == 0) return false;

        if (!GetWalkability(targetX, targetY)) return false;

        if (onClosedList > 1000000)
        {
            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++) whichList[x, y] = 0;
            onClosedList = 10;
        }
        onClosedList = onClosedList + 2;
        onOpenList = onClosedList - 1;
        pathLength = 0;
        pathLocation = 0;
        Gcost[startX, startY] = 0;

        numberOfOpenListItems = 1;
        openList[1] = 1;
        openX[1] = startX; openY[1] = startY;

        do
        {
            if (numberOfOpenListItems != 0)
            {
                parentXval = openX[openList[1]];
                parentYval = openY[openList[1]];
                whichList[parentXval, parentYval] = onClosedList;

                numberOfOpenListItems = numberOfOpenListItems - 1;

                openList[1] = openList[numberOfOpenListItems + 1];
                v = 1;

                do
                {
                    u = v;
                    if (2 * u + 1 <= numberOfOpenListItems)
                    {
                        if (Fcost[openList[u]] >= Fcost[openList[2 * u]]) v = 2 * u;
                        if (Fcost[openList[v]] >= Fcost[openList[2 * u + 1]]) v = 2 * u + 1;
                    }
                    else
                    {
                        if (2 * u <= numberOfOpenListItems)
                            if (Fcost[openList[u]] >= Fcost[openList[2 * u]]) v = 2 * u;
                    }

                    if (u != v)
                    {
                        temp = openList[u];
                        openList[u] = openList[v];
                        openList[v] = temp;
                    }
                    else break;

                } while (true);


                for (b = parentYval - 1; b <= parentYval + 1; b++)
                {
                    for (a = parentXval - 1; a <= parentXval + 1; a++)
                    {

                        if (a != -1 && b != -1 && a != mapWidth && b != mapHeight)
                        {

                            if (whichList[a, b] != onClosedList)
                            {

                                if (GetWalkability(a, b))
                                {

                                    corner = true;
                                    if (a == parentXval - 1)
                                    {
                                        if (b == parentYval - 1)
                                        {
                                            if (!GetWalkability(parentXval - 1, parentYval) ||
                                                !GetWalkability(parentXval, parentYval - 1))
                                                corner = false;
                                        }
                                        else if (b == parentYval + 1)
                                        {
                                            if (!GetWalkability(parentXval, parentYval + 1) ||
                                                !GetWalkability(parentXval - 1, parentYval))
                                                corner = false;
                                        }
                                    }
                                    else if (a == parentXval + 1)
                                    {
                                        if (b == parentYval - 1)
                                        {
                                            if (!GetWalkability(parentXval, parentYval - 1) ||
                                                !GetWalkability(parentXval + 1, parentYval))
                                                corner = false;
                                        }
                                        else if (b == parentYval + 1)
                                        {
                                            if (!GetWalkability(parentXval + 1, parentYval) ||
                                                !GetWalkability(parentXval, parentYval + 1))
                                                corner = false;
                                        }
                                    }

                                    if (corner == true)
                                    {

                                        if (whichList[a, b] != onOpenList)
                                        {

                                            newOpenListItemID = newOpenListItemID + 1;
                                            m = numberOfOpenListItems + 1;
                                            openList[m] = newOpenListItemID;
                                            openX[newOpenListItemID] = a;
                                            openY[newOpenListItemID] = b;

                                            if (Mathf.Abs(a - parentXval) == 1 && Mathf.Abs(b - parentYval) == 1) addedGCost = 14;
                                            else addedGCost = 10;
                                            Gcost[a, b] = Gcost[parentXval, parentYval] + addedGCost;

                                            Hcost[openList[m]] = 10 * (Mathf.Abs(a - targetX) + Mathf.Abs(b - targetY));
                                            Fcost[openList[m]] = Gcost[a, b] + Hcost[openList[m]];
                                            parentX[a, b] = parentXval;
                                            parentY[a, b] = parentYval;

                                            while (m != 1)
                                            {
                                                if (Fcost[openList[m]] <= Fcost[openList[m / 2]])
                                                {
                                                    temp = openList[m / 2];
                                                    openList[m / 2] = openList[m];
                                                    openList[m] = temp;
                                                    m = m / 2;
                                                }
                                                else break;
                                            }
                                            numberOfOpenListItems = numberOfOpenListItems + 1;

                                            whichList[a, b] = onOpenList;
                                        }
                                        else
                                        {

                                            if (Mathf.Abs(a - parentXval) == 1 && Mathf.Abs(b - parentYval) == 1) addedGCost = 14;
                                            else addedGCost = 10;
                                            tempGcost = Gcost[parentXval, parentYval] + addedGCost;

                                            if (tempGcost < Gcost[a, b])
                                            {
                                                parentX[a, b] = parentXval;
                                                parentY[a, b] = parentYval;
                                                Gcost[a, b] = tempGcost;

                                                for (int x = 1; x <= numberOfOpenListItems; x++)
                                                {
                                                    if (openX[openList[x]] == a && openY[openList[x]] == b) //item found
                                                    {
                                                        Fcost[openList[x]] = Gcost[a, b] + Hcost[openList[x]];
                                                        m = x;
                                                        while (m != 1)
                                                        {
                                                            if (Fcost[openList[m]] < Fcost[openList[m / 2]])
                                                            {
                                                                temp = openList[m / 2];
                                                                openList[m / 2] = openList[m];
                                                                openList[m] = temp;
                                                                m = m / 2;
                                                            }
                                                            else
                                                                break;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            else { found = false; break; }

            if (whichList[targetX, targetY] == onOpenList) { found = true; break; }

        }
        while (true);

        if (found)
        {
            path.found = true;
            pathX = targetX; pathY = targetY;
            do
            {
                tempx = parentX[pathX, pathY];
                pathY = parentY[pathX, pathY];
                pathX = tempx;
                pathLength = pathLength + 1;
            }
            while (pathX != startX || pathY != startY);

            path.length = pathLength + 1;
            path.data = new PATile[pathLength + 1];
            path.data[0] = GetTile(startX, startY);
            pathX = targetX; pathY = targetY;
            int ind = pathLength;

            do
            {
                path.data[ind] = GetTile(pathX, pathY);
                ind--;

                tempx = parentX[pathX, pathY];
                pathY = parentY[pathX, pathY];
                pathX = tempx;
            }
            while (pathX != startX || pathY != startY);
        }
        return found;
    }
}