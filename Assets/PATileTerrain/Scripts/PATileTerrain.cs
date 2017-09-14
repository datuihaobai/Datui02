/* ===========================================================
 *  PATileTerrain.cs
 *  Copyright (C) 2011-2012, Pozdnyakov Anton. 
 * v1.04
 * =========================================================== */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using PathologicalGames;

public partial class PATileTerrain: MonoBehaviour 
{
	public PATile GetTile(int x, int y) { return settings.tiles[settings.xCount * y + x]; }
	public PATile GetTile(int index) { return settings.tiles[index]; }
	public PATileTerrainChunk GetChunk(int x, int y) { return settings.chunks[settings.chunkCountX * y + x]; }
	public PATileTerrainChunk GetChunk(int index) { return settings.chunks[index]; }
	
	public float GetHeight(Vector3 pos)
	{
		pos = pos - transform.position;
		
		if (pos.x >= 0 && pos.x < settings.xCount * settings.tileSize &&
		    pos.z >= 0 && pos.z < settings.yCount * settings.tileSize)
		{
			int x = (int)(pos.x / settings.tileSize);
        	int y = (int)(pos.z / settings.tileSize);
			
			float xt = pos.x - x * settings.tileSize;
        	float yt = pos.z - y * settings.tileSize;
			
			Vector3 intersection = Vector3.zero;
			Vector3 pointA, pointB, pointC;
			if (xt <= yt)
        	{
				pointA = new Vector3(0.0f, GetPointHeight(x, y), 0.0f);
            	pointB = new Vector3(0.0f, GetPointHeight(x, y + 1), settings.tileSize);
            	pointC = new Vector3(settings.tileSize, GetPointHeight(x + 1, y + 1), settings.tileSize);
			} else
			{
				pointA = new Vector3(0, GetPointHeight(x, y), 0);
            	pointB = new Vector3(settings.tileSize, GetPointHeight(x + 1, y + 1), settings.tileSize);
            	pointC = new Vector3(settings.tileSize, GetPointHeight(x + 1, y), 0.0f);
			}
			
			Vector3 linePoint = new Vector3(xt, 0.0f, yt);
			Vector3 lineNormal = (Vector3.Cross((pointB - pointA), (pointC - pointA))).normalized;
			float t2 = Vector3.Dot(lineNormal, Vector3.up); 
			
			if (t2 != 0)
			{
				float d = Vector3.Dot(pointA, lineNormal);
				float t = - (Vector3.Dot(lineNormal, linePoint) - d) / t2;
				
				intersection = linePoint + Vector3.up * t;
				
				if (IsOnSameSide(intersection, pointA, pointB, pointC) &&
					IsOnSameSide(intersection, pointB, pointA, pointC) &&
					IsOnSameSide(intersection, pointC, pointA, pointB))
				{
					return intersection.y + transform.position.y;
				}
			}
			
			return intersection.y + transform.position.y;
			
		}
		
		return 0.0f;
	}
		
	//Edit Methods 
	/*public PATile GetTileByTriangleIndex(int triangleIndex)
	{
		//Triangle index to tile index
		if ((triangleIndex & 0x01) == 1) triangleIndex--;
		triangleIndex /= 2;		
		return settings.tiles[triangleIndex];
	}*/
	
	public Mesh GetChunkMesh(int chunk) { return settings.chunks[chunk].settings.mesh; }
	
	public void UpdateMesh()
	{
		RecalculateNormals();
		foreach (PATileTerrainChunk c in settings.chunks)
		{
			c.settings.mesh.RecalculateBounds();
			c.gameObject.GetComponent<MeshCollider>().enabled = false;
			c.gameObject.GetComponent<MeshCollider>().enabled = true;
		}
		
	}
	
	public void UpdateMesh(List<PAPointXY> points)
	{ 		
		RecalculateNormals(points);
		foreach (PATileTerrainChunk c in settings.chunks)
		{
			c.settings.mesh.RecalculateBounds();
			c.gameObject.GetComponent<MeshCollider>().enabled = false;
			c.gameObject.GetComponent<MeshCollider>().enabled = true;
		}
	}
	
	public float GetPointHeight(int x, int y)
	{								
		PATile tile;
		Mesh mesh;
		PAPoint point = settings.points[(settings.xCount + 1) * y + x];
		for (int i = 0; i < 4; ++i)
			if (point.t[i] >= 0)
			{
				tile = GetTile(point.t[i]);
				mesh = GetChunkMesh(tile.chunkId);
				return mesh.vertices[tile.cId * 4 + point.p[i]].y;
				//return GetTile(point.t[i]).verts[point.p[i]].y;
			}	
		return 0.0f;
	}
	
	public void SetPointHeight(int x, int y, float h, bool a)
	{
		PATile tile;
		int i, j, id;
		bool c;
		List<Mesh> ms = new List<Mesh>();
		List<Vector3[]> vs = new List<Vector3[]>();
		
		Mesh mesh;
		Vector3[] vertices;
		PAPoint point = settings.points[(settings.xCount + 1) * y + x];
		for (i = 0; i < 4; ++i)
			if (point.t[i] >= 0)
			{
				c = false;
				tile = GetTile(point.t[i]);
				mesh = GetChunkMesh(tile.chunkId);
				vertices = null;
				for (j = 0; j < ms.Count; ++j) 
                    if (ms[j] == mesh) 
                    { c = true; vertices = vs[j]; }
				if (vertices == null) 
                    vertices = mesh.vertices;
			
				id = tile.cId * 4 + point.p[i];
				if (a) 
                    vertices[id].y += h; 
                else 
                    vertices[id].y = h;
				vertices[id].y = Mathf.Clamp(vertices[id].y, settings.minHeight, settings.maxHeight);
				
				if (!c) { ms.Add(mesh); vs.Add(vertices); }
			}	
		for (i = 0; i < ms.Count; ++i) 
            ms[i].vertices = vs[i];
	}
	
	public void SetPointNormal(int x, int y, Vector3 n)
	{
		PATile tile;
		int i, j;
		bool c;
		List<Mesh> ms = new List<Mesh>();
		List<Vector3[]> ns = new List<Vector3[]>();
		
		Mesh mesh;
		Vector3[] normals;
		PAPoint point = settings.points[(settings.xCount + 1) * y + x];
		for (i = 0; i < 4; ++i)
			if (point.t[i] >= 0)
			{
				c = false;
				tile = GetTile(point.t[i]);			
							
				mesh = GetChunkMesh(tile.chunkId);
				normals = null;
				for (j = 0; j < ms.Count; ++j) if (ms[j] == mesh) { c = true; normals = ns[j]; }
				if (normals == null) normals = mesh.normals;
			
				normals[tile.cId * 4 + point.p[i]] = n;
			
				if (!c) { ms.Add(mesh); ns.Add(normals); }
			}
			
		for (i = 0; i < ms.Count; ++i) ms[i].normals = ns[i];
	}
	
	public void RecalculateNormals(List<PAPointXY> points)
	{
		for (int i = 0; i < points.Count; ++i) RecalculatePointNormal(points[i].x, points[i].y);
	}
	
	public void RecalculateNormals()
	{
		for (int y = 0; y <= settings.yCount; ++y)	
			for (int x = 0; x <= settings.xCount; ++x) RecalculatePointNormal(x, y);		
	}
	
	public void RecalculatePointNormal(int x, int y)
	{		
	    Vector3 v0, v1;
	    Vector3 n0, n1, n2, n3, n4, n5;
				
	    //(0, -1)
	    if (y > 0) v0 = new Vector3(0.0f, GetPointHeight(x, y - 1) - GetPointHeight(x, y), -settings.tileSize);
	    else v0 = new Vector3(0.0f, 0.0f, -settings.tileSize);
		
	    //(-1, -1)
	    if(x > 0 && y > 0) v1 = new Vector3(-settings.tileSize, GetPointHeight(x - 1, y - 1) - GetPointHeight(x, y), -settings.tileSize);
	    else v1 = new Vector3(-settings.tileSize, 0.0f, -settings.tileSize);
	    n0 = Vector3.Cross(v0, v1);
	
	    //(-1, -1)
	    v0 = v1;
	    //(-1, 0)
	    if (x > 0) v1 = new Vector3(-settings.tileSize, GetPointHeight(x - 1, y) - GetPointHeight(x, y), 0.0f);
	    else v1 = new Vector3(-settings.tileSize, 0.0f, 0.0f);
	    n1 = Vector3.Cross(v0, v1);
	
	    //(-1, 0)
	    v0 = v1;
	    //(0, 1)
	    if (y < settings.yCount) v1 = new Vector3(0.0f, GetPointHeight(x, y + 1) - GetPointHeight(x, y), settings.tileSize);
	    else v1 = new Vector3(0.0f, 0.0f, settings.tileSize);
	    n2 = Vector3.Cross(v0, v1);
	
	    //(0, 1)
	    v0 = v1;
	    //(1, 1)
	    if (x < settings.xCount && y < settings.yCount) v1 = new Vector3(settings.tileSize, GetPointHeight(x + 1, y + 1) - GetPointHeight(x, y), settings.tileSize);
	    else v1 = new Vector3(settings.tileSize, 0.0f, settings.tileSize);
	    n3 = Vector3.Cross(v0, v1);
	
	    //(1, 1)
	    v0 = v1;
	    //(1, 0)
	    if (x < settings.xCount) v1 = new Vector3(settings.tileSize, GetPointHeight(x + 1, y) - GetPointHeight(x, y), 0.0f);
	    else v1 = new Vector3(settings.tileSize, 0.0f, 0.0f);
	    n4 = Vector3.Cross(v0, v1);
	
	    //(1, 0)
	    v0 = v1;
	    //(0, -1)
	    if (y > 0) v1 = new Vector3(0.0f, GetPointHeight(x, y - 1) - GetPointHeight(x, y), -settings.tileSize);
	    else v1 = new Vector3(0.0f, 0.0f, -settings.tileSize);
	    n5 = Vector3.Cross(v0, v1);
	
	    Vector3 m0, m1, m2, m3;
	    m0 = (n1 - n0) / 2 + n0;
	    m1 = n2;
	    m2 = (n4 - n3) / 2 + n3;
	    m3 = n5;
		
	    Vector3 k0, k1;
	    k0 = (m2 - m0) / 2 + m0;
	    k1 = (m3 - m1) / 2 + m1;
	
	    Vector3 n = (k1 - k0) / 2 + k0;
	    n = n.normalized;
		
		SetPointNormal(x, y, n);
	}
	
	public void SmoothPointTerrain(int x, int y, bool rect, float power, float radius, List<PAPointXY> points)
	{		
		int i, j, pi, mi;
		int count = (int)(radius / settings.diagonalLength);
		int xMin = x, xMax = x, yMin = y, yMax = y;
		float h;
		Vector3 currentPos, tilePos;
		PAPointXY p;
		PATile tile;
		List<Mesh> ms = new List<Mesh>();
		List<Vector3[]> vs = new List<Vector3[]>();
		Mesh mesh;
		bool c;
		PAPoint point;
		Vector3[] vertices;		
		
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
			h = GetSmoothPointHeight(i, j, power);
			
			point = settings.points[(settings.xCount + 1) * j + i];
			for (pi = 0; pi < 4; ++pi)
				if (point.t[pi] >= 0)
				{
					c = false;
					tile = GetTile(point.t[pi]);
					mesh = GetChunkMesh(tile.chunkId);
					vertices = null;
					
					for (mi = 0; mi < ms.Count; ++mi) if (ms[mi] == mesh) { c = true; vertices = vs[mi]; }
					if (vertices == null) vertices = mesh.vertices;
				
					vertices[tile.cId * 4 + point.p[pi]].y = Mathf.Clamp(h, settings.minHeight, settings.maxHeight);
	
					if (!c) { ms.Add(mesh); vs.Add(vertices); }
				}	
		}
		
		for (i = 0; i < ms.Count; ++i) ms[i].vertices = vs[i];
	}
	
	public float GetSmoothPointHeight(int x, int y, float p)
	{			
		PAPoint point = settings.points[(settings.xCount + 1) * y + x];
		PATile tile = GetTile(point.t[0]);		
		//float h = GetTile(point.t[0]).verts[point.p[0]].y;
		float h = GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y;
		float hh = h, hs = h, hr, hp;
		int hc = 1;		
		
		if (x > 0)  //L
		{			
			point = settings.points[(settings.xCount + 1) * y + x - 1];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}	
		if (y < settings.yCount) //U
		{
			point = settings.points[(settings.xCount + 1) * (y + 1) + x];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}
		if (x < settings.xCount) //R
		{
			point = settings.points[(settings.xCount + 1) * y + x + 1];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}			
		if (y > 0) //D
		{
			point = settings.points[(settings.xCount + 1) * (y - 1) + x];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}
		if (x > 0 && y < settings.yCount) //LU
		{
			point = settings.points[(settings.xCount + 1) * (y + 1) + x - 1];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}
		if (x > 0 && y > 0) //LD
		{
			point = settings.points[(settings.xCount + 1) * (y - 1) + x - 1];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}
		if (x < settings.xCount && y < settings.yCount) //RU
		{
			point = settings.points[(settings.xCount + 1) * (y + 1) + x + 1];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}
		if (x < settings.xCount && y > 0) //RD
		{
			point = settings.points[(settings.xCount + 1) * (y - 1) + x + 1];
			tile = GetTile(point.t[0]);
			h += GetChunkMesh(tile.chunkId).vertices[tile.cId * 4 + point.p[0]].y; hc++;
			//h += GetTile(point.t[0]).verts[point.p[0]].y; hc++;
		}		
		
		hr = h / hc;		
		if (hs > hr) hh = hs - hr;
		else hh = hr - hs;
		if (hs > hr) hp = hs - p * hh;
		else  hp = hs + p * hh;			
		
		//SetPointHeight(x, y, hp, false);	
		return hp;
	}	
	
	public void DeformPointTerrain(int x, int y, bool rect, float power, float radius, List<PAPointXY> points)
	{		
		int count = (int)(radius / settings.diagonalLength);
		int xMin = x, xMax = x, yMin = y, yMax = y;
		int i, j;
		float distance, falloff;
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
		        p = new PAPointXY(i, j); if (!points.Contains(p)) points.Add(p);			
		        if (rect)
		        {
			        falloff = power;
		        } else
		        {
			        tilePos = new Vector3(i * settings.tileSize, 0.0f, j * settings.tileSize);
			        distance = Vector3.Distance(currentPos, tilePos);
			        falloff = GaussFalloff(distance, radius) * power;
		        }
		        SetPointHeight(i, j, falloff, true);
	        }
	}
	
	//Painting	
	protected PATileUV GetIndexUV(int index)
	{
		PATileUV uv = new PATileUV();
		
		int x, y;
		
		y = index / settings.tilesetX;
		x = index - y * settings.tilesetX;
		y = settings.tilesetY - y - 1; //(0,0) in left-bottom	
		
		uv.p0 = new Vector2(x * settings.tilesetWidth + settings.uvOffset.x, 
		                    y * settings.tilesetHeight + settings.tilesetHeight - settings.uvOffset.y); 
		uv.p1 = new Vector2(x * settings.tilesetWidth + settings.tilesetWidth -settings.uvOffset.x, 
		                    y * settings.tilesetHeight + settings.tilesetHeight - settings.uvOffset.y); 
		uv.p2 = new Vector2(x * settings.tilesetWidth + settings.tilesetWidth - settings.uvOffset.x, 
		                    y * settings.tilesetHeight + settings.uvOffset.y);  
		uv.p3 = new Vector2(x * settings.tilesetWidth + settings.uvOffset.x, 
		                    y * settings.tilesetHeight + settings.uvOffset.y);
		return uv;
	}

    public PATile[] GetCrystalTileNeighbor(PACrystalTile crystalTile,int n)
    {
        return GetCrystalTileNeighbor(crystalTile.leftBottomTile.x,crystalTile.leftBottomTile.y,n);
    }

    public PATile[] GetCrystalTileNeighbor(int x,int y ,int n)
    {
        int nc = n + 3;
        int ct = nc * nc - (n + 1) * (n + 1);

        PATile[] t = new PATile[ct];
        for (int i = 0; i < ct; ++i) t[i] = null;

        int index = 0;
        //◊Û
        for (int iy = y - 1; iy < y + 3; iy ++)
        {
            if (x < 1) { index++; continue; }
            if (iy < 0 || iy > settings.yCount - 1) { index++; continue; }

            t[index++] = GetTile(x-1,iy);
        }
        //…œ
        for (int ix = x; ix < x + 3; ix ++)
        {
            if (y + 2 > settings.yCount - 1) { index++; continue; }
            if (ix > settings.xCount - 1) { index++; continue; }

            t[index++] = GetTile(ix,y+2);
        }
        //”“
        for (int iy = y + 1; iy > y - 2;iy--)
        {
            if (x + 2 > settings.xCount - 1) { index++; continue; }
            if (iy < 0 || iy > settings.yCount - 1) { index++; continue; }

            t[index++] = GetTile(x+2,iy);
        }
        //œ¬
        for (int ix = x + 1; ix > x - 1; ix--)
        {
            if (y - 1 < 0) { index++; continue; }
            if (ix > settings.xCount - 1) { index++; continue; }

            t[index++] = GetTile(ix,y-1);
        }

        return t;
    }

	public PATile[] GetNeighboringTilesNxN(PATile tile, int n) //n must be 1,3,5,7,9,11... etc
	{ return GetNeighboringTilesNxN(tile.x, tile.y, n); }
	
	public PATile[] GetNeighboringTilesNxN(int x, int y, int n) //n must be 1,3,5,7,9,11... etc
	{
		//Universal algorithm to search for nearby tiles
		
		//n = 1 ---> 1x1 - 8
		//Array visualization, where 'x' = current tile
		// 0  1  2
		// 7  x  3
		// 6  5  4
			
		//n = 3 ---> 3x3 - 16
		//Array visualization, where 'c' = current tile
		// 0  1  2  3  4
		//15  x  x  x  5
		//14  x  c  x  6
		//13  x  x  x  7 
		//12 11 10  9  8 	
		
		//n = 5 ---> 5x5 - 24
		//Array visualization, where 'c' = current tile
		// 0  1  2  3  4  5  6
		// 23 x  x  x  x  x  7
		// 22 x  x  x  x  x  8
		// 21 x  x  c  x  x  9
		// 20 x  x  x  x  x  10
		// 19 x  x  x  x  x  11
		// 18 17 16 15 14 13 12
		
		int i;
		int nc = n + 2;
		int ct = nc * nc - n * n; 
		int m = (nc - 1) / 2, s;
		
		PATile[] t = new PATile[ct]; 
		for (i = 0; i < ct; ++i) t[i] = null;
		
		//up
		if (x > (m - 1)) 
		{
			//left and center
			for (i = -1; i < m; ++i)
				if (y > i) t[m - (i + 1)] = GetTile(x - m, y - (i + 1));
			//right
			for (i = 1; i <= m; ++i)
				if (y < settings.yCount - i) t[m + i] = GetTile(x - m, y + i);
		}
		
		//bottom
		if (x < settings.xCount - m)
		{
			//right and center
			s = nc + n;
			for (i = m; i >= 0; --i) 
				if (y < settings.yCount - i) t[s + m - i] = GetTile(x + m, y + i);
			//left
			s += m + 1;
			for (i = 0; i < m; ++i)
				if (y > i) t[s + i] = GetTile(x + m, y - (i + 1));
		}
		
		//right
		if (y < settings.yCount - m)
		{
			//up
			for (i = m - 2; i >= -1; --i) 
				if (x > i) t[n + m - i]	= GetTile(x - (i + 1), y + m);
			//bottom
			for (i = 1; i < m; ++i) 
				if (x < settings.xCount - i) t[nc + m + i - 1] = GetTile(x + i, y + m);
		}
		
		//left
		if (y > m - 1)
		{
			//bottom
			s = ct - n;
			for (i = m - 1; i >= 0; --i)
				if (x < settings.xCount - i) t[s + m - 1 - i] = GetTile(x + i, y - m);
			//up
			s = ct - m + 1;
			for (i = 0; i < m - 1; ++i)
				if (x > i) t[s + i] = GetTile(x - (i + 1), y - m);
		} 
		
		return t;
	}

	protected void CheckTile(PATile tile)
	{
		if (tile != null && tile.type >= settings.tsTypes.Count) { tile.type = -1; tile.bits = 0; }
	}

    public void PaintCrystalTile(PATile tile,int t , int range)
    {
        if (tile == null) return;

        t = Mathf.Clamp(t, 0, settings.tsTypes.Count);

        IntermediateInfo[] imInfo = new IntermediateInfo[(range + 3) * (range + 3) - (range + 1) * (range + 1)];
        List<PATile> normalTiles = new List<PATile>();
        PACrystalTile crystalTile = PACrystalTile.GetByTile(this,tile);

        //for (int i = 1; i < range - 1; i += 2)
        //    normalTiles.AddRange(GetNeighboringTilesNxN(tile, i));

        if (tile.type == tile.toType && tile.type == t && tile.bits == 0)
        { }
        else
        {
            tile.type = t;
            tile.toType = t;
            tile.bits = 0;
            UpdateTileUV(tile);
        }

        foreach (var normalTile in normalTiles)
        {
            if (normalTile.type == normalTile.toType && normalTile.type == t && normalTile.bits == 0)
            { }
            else
            {
                normalTile.type = t;
                normalTile.toType = t;
                normalTile.bits = 0;
                UpdateTileUV(normalTile);
            }
        }

        PATile[] otherTiles = crystalTile.GetOtherTiles(this);
        foreach(var otherTile in otherTiles)
        {
            if (otherTile.type == otherTile.toType && otherTile.type == t && otherTile.bits == 0)
            { }
            else
            {
                otherTile.type = t;
                otherTile.toType = t;
                otherTile.bits = 0;
                UpdateTileUV(otherTile);
            }
        }

        PATile[] nTiles = GetCrystalTileNeighbor(crystalTile, range);
        int k = 0;
        //◊Ûœ¬Ω«
        CalcTileBits(t, nTiles[k], 2, out imInfo[k++]);
        //◊Û±ﬂ
        for (int j = 0; j < range + 1; j++)
            CalcTileBits(t, nTiles[k], 3, out imInfo[k++]);
        //◊Û…œΩ«
        CalcTileBits(t, nTiles[k], 1, out imInfo[k++]);
        //…œ±ﬂ
        for (int j = 0; j < range + 1; j++)
            CalcTileBits(t, nTiles[k], 9, out imInfo[k++]);
        //”“…œΩ«	
        CalcTileBits(t, nTiles[k], 8, out imInfo[k++]);
        //”“±ﬂ
        for (int j = 0; j < range + 1; j++)
            CalcTileBits(t, nTiles[k], 12, out imInfo[k++]);
        //”“œ¬Ω«
        CalcTileBits(t, nTiles[k], 4, out imInfo[k++]);
        //œ¬±ﬂ
        for (int j = 0; j < range + 1; j++)
            CalcTileBits(t, nTiles[k], 6, out imInfo[k++]);
    }

    public void PaintTile(PATile tile, int t,int range)
    {
        if (tile == null) return;
        
        t = Mathf.Clamp(t, 0, settings.tsTypes.Count);

        IntermediateInfo[] imInfo = new IntermediateInfo[(range+2)*(range+2)-range*range];
        List<PATile> normalTiles = new List<PATile>();

        for (int i = 1; i < range - 1; i += 2)
            normalTiles.AddRange(GetNeighboringTilesNxN(tile, i));

        if (tile.type == tile.toType && tile.type == t && tile.bits == 0)
        { }
        else
        {
            tile.type = t;
            tile.toType = t;
            tile.bits = 0;
            UpdateTileUV(tile);
        }

        foreach(var normalTile in normalTiles)
        {
            if (normalTile.type == normalTile.toType && normalTile.type == t && normalTile.bits == 0)
            { }
            else
            {
                normalTile.type = t;
                normalTile.toType = t;
                normalTile.bits = 0;
                UpdateTileUV(normalTile);
            }
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

        PATile[] nTiles = GetNeighboringTilesNxN(tile, range);
        //PATile[] ncTiles = GetCrystalTileNeighbor(tile.x,tile.y, range);
        int k = 0;
        //◊Ûœ¬Ω«
        CalcTileBits(t, nTiles[k], 2, out imInfo[k++]);
        //◊Û±ﬂ
        for (int j = 0; j < range; j++)
            CalcTileBits(t, nTiles[k], 3, out imInfo[k++]);
        //◊Û…œΩ«
        CalcTileBits(t, nTiles[k], 1, out imInfo[k++]);
        //…œ±ﬂ
        for (int j = 0; j < range; j++)
            CalcTileBits(t, nTiles[k], 9, out imInfo[k++]);
        //”“…œΩ«	
        CalcTileBits(t, nTiles[k], 8, out imInfo[k++]);
        //”“±ﬂ
        for (int j = 0; j < range; j++)
            CalcTileBits(t, nTiles[k], 12, out imInfo[k++]);
        //”“œ¬Ω«
        CalcTileBits(t, nTiles[k], 4, out imInfo[k++]);
        //œ¬±ﬂ
        for (int j = 0; j < range; j++)
            CalcTileBits(t, nTiles[k], 6, out imInfo[k++]);
    }

    protected void CalcIntermediateTileBits(PATile tile, byte b, IntermediateInfo imInfo)
	{
		if (tile == null || imInfo == null) return;
		
		IntermediateInfo imInfoInternal;		
		CalcTileBits(imInfo.imToType, tile, b, out imInfoInternal);
	}
	
    protected void CalcTileBits(int t, PATile tile, byte b)
    {
        if (tile == null)
            return;

        //t = tile.element.GetPaintBrushType();
        IntermediateInfo imInfo;
        CalcTileBits(t,tile,b,out imInfo);
    }

	protected void CalcTileBits(int t, PATile tile, byte b, out IntermediateInfo imInfo)
	{
		imInfo = null;
		if (tile == null) return;
		
		PATSTransition transition = null;
		bool invertedBits, needIntermediate = false;
		byte bits = tile.bits;
		bool isNull = (bits == 0 || tile.type == tile.toType);
		int fromType = t, toType = t;	
		
		if (isNull)
		{			
			if (fromType != tile.type)
			{
				transition = FindTransition(fromType, tile.type);
				toType = tile.type;
				if (transition != null)
				{
					invertedBits = (fromType == transition.to);
					bits = b;
					if (invertedBits) bits = InvertBits(bits);	
					
				} else 
				{
					needIntermediate = true;
				}
			} else if (fromType == tile.type)
			{
				//nothing to do
				toType = fromType; bits = 0;
			}
			
		} else
		{
			if (fromType == tile.type) 
			{
				transition = FindTransition(fromType, tile.toType);
				toType = tile.toType;
			}
			else if (fromType == tile.toType) 
			{
				transition = FindTransition(fromType, tile.type);
				toType = tile.type;
			} else 
			{
				needIntermediate = true;
				toType = tile.type;
			}
			
			if (transition != null)
			{
				invertedBits = (fromType == transition.to);
				
				//if (fromType == tile.type)
					// -----       -----
					// |1|2|  ---  | 1 |
					// -----       -----
				//else if (fromType == tile.toType)
					// -----       -----
					// |2|1|  ---  | 1 |
					// -----       -----				
				
				if (invertedBits) bits = InvertBits(bits);
				bits = (byte)(bits | b);
				if (invertedBits) bits = InvertBits(bits);
				if (bits == 15) { bits = 0; toType = fromType; }
			}
					
		}
		
		//Intermediate transition
		if (needIntermediate)
		{
			imInfo = new IntermediateInfo();
			imInfo.fromType = fromType;
			imInfo.toType = toType;
			
			transition = FindIntermediateTransition(fromType, toType);
			if (transition != null)
			{
				//Debug.Log("Intermediate transition = " + transition.name);
				if (fromType == transition.from) toType = transition.to;
				else if (fromType == transition.to) toType = transition.from;
					
				invertedBits = (fromType == transition.to);
				bits = b;
				if (invertedBits) bits = InvertBits(bits);
				if (bits == 15) { bits = 0; toType = fromType; }
				
				imInfo.imFromType = fromType;
				imInfo.imToType = toType;
			} else 
			{
				Debug.LogError("Not found transition between '"+ settings.tsTypes[fromType].name +"' and '" + settings.tsTypes[toType].name + "'!");
				imInfo = null;
			}
		}			
		
		tile.type = fromType;
		tile.toType = toType;
		tile.bits = bits;
		UpdateTileUV(tile);
	}

    public enum UVRotateType
    {
        None = 0,
        _90 = 1,
        _180 = 2,
        _270 = 3,

        Mirror_LR = 4,
        Mirror_TB = 5,
    }

    protected void UpdateTileUV(PATile tile, int specifiedIndex = -1,UVRotateType rotateType = UVRotateType.None)
	{
		if (tile.type == -1) { return; }	

		Mesh mesh = GetChunkMesh(tile.chunkId);
		Vector2[] uvs = mesh.uv;
		int i = tile.cId;
		int index;

		if (tile.bits == 0 || tile.type == tile.toType)
		{
			PATSType type = settings.tsTypes[tile.type];
			//index = type.baseIndexes[0];
            index = type.GetRandomBaseIndex();
			tile.bits = 0;
		} else
		{			
			int id = FindTransitionBitsId(tile.bits);
			int transitionId;			
			
			PATSTransition transition = FindTransition(tile.type, tile.toType, out transitionId);	
			if (transition == null)
			{
				Debug.LogError("For the tile set is not known transition!");
				PATSType type = settings.tsTypes[tile.type];
				//index = type.baseIndexes[0];
                index = type.GetRandomBaseIndex();
			} else 
			{
				index = transition.transitions[id];
			}
		}

        if (specifiedIndex != -1)
            index = specifiedIndex;

        //tile.tilesetIndex = index;

        PATileUV uv = GetIndexUV(index);

        if(rotateType == UVRotateType.None)
        {
            uvs[i * 4 + 0] = uv.p0;
            uvs[i * 4 + 1] = uv.p1;
            uvs[i * 4 + 2] = uv.p2;
            uvs[i * 4 + 3] = uv.p3;
        }
        else if (rotateType == UVRotateType._90)
        {
            uvs[i * 4 + 0] = uv.p3;
            uvs[i * 4 + 1] = uv.p0;
            uvs[i * 4 + 2] = uv.p1;
            uvs[i * 4 + 3] = uv.p2;
        }
        else if (rotateType == UVRotateType._180)
        {
            uvs[i * 4 + 0] = uv.p2;
            uvs[i * 4 + 1] = uv.p3;
            uvs[i * 4 + 2] = uv.p0;
            uvs[i * 4 + 3] = uv.p1;
        }
        else if (rotateType == UVRotateType._270)
        {
            uvs[i * 4 + 0] = uv.p1;
            uvs[i * 4 + 1] = uv.p2;
            uvs[i * 4 + 2] = uv.p3;
            uvs[i * 4 + 3] = uv.p0;
        }
        else if (rotateType == UVRotateType.Mirror_LR)
        {
            uvs[i * 4 + 0] = uv.p1;
            uvs[i * 4 + 1] = uv.p0;
            uvs[i * 4 + 2] = uv.p3;
            uvs[i * 4 + 3] = uv.p2;
        }
        else if (rotateType == UVRotateType.Mirror_TB)
        {
            uvs[i * 4 + 0] = uv.p3;
            uvs[i * 4 + 1] = uv.p2;
            uvs[i * 4 + 2] = uv.p1;
            uvs[i * 4 + 3] = uv.p0;
        }
		
		mesh.uv = uvs;
	}
	
	public void FillTerrain(int t)
	{		
		PATile tile;
		PATSType type = settings.tsTypes[t];
		//int transition = FindTransitionId(t);
		//PATileUV uv = GetIndexUV(type.baseIndexes[0]);
		Mesh mesh;
		Vector2[] uvs;
		for (int i = 0; i < settings.xCount * settings.yCount; ++i)
		{
			tile = settings.tiles[i];
			mesh = GetChunkMesh(tile.chunkId);
			uvs = mesh.uv;
			
			tile.type = t;
			tile.toType = t;
			tile.bits = 0;	
            int index = type.GetRandomBaseIndex();
            //tile.tilesetIndex = index;
            PATileUV uv = GetIndexUV(index);
			uvs[tile.cId * 4 + 0] = uv.p0;//tile.uvs[0];
			uvs[tile.cId * 4 + 1] = uv.p1;//tile.uvs[1];
			uvs[tile.cId * 4 + 2] = uv.p2;//tile.uvs[2];
			uvs[tile.cId * 4 + 3] = uv.p3;//tile.uvs[3];
			
			mesh.uv = uvs;
		}
	}
		
	public void CreateTerrain(JSONNode jsnode = null)
	{
		DestroyTerrain();

        if (jsnode == null)
        {
            settings.name = editorSettings.name;
            settings.xCount = editorSettings.x;
            settings.yCount = editorSettings.y;
            settings.chunkSize = editorSettings.chunkSize;
            settings.tileSize = editorSettings.tileSize;
            settings.minHeight = editorSettings.minHeight;
            settings.maxHeight = editorSettings.maxHeight;
        }
        else
        {
            settings.FromJson(jsnode);
        }
        settings.diagonalLength = Mathf.Sqrt(Mathf.Pow(settings.tileSize, 2) + Mathf.Pow(settings.tileSize, 2));
		
        if(jsnode == null)
        {
            settings.tilesetX = 1;
            settings.tilesetY = 1;
            settings.tilesetCount = 1;
            settings.tilesetWidth = 1.0f;
            settings.tilesetHeight = 1.0f;
            settings.tilesetMaterial = editorSettings.tileSetMaterial;
        }
        else
        {
            RecalcTilesetSizes();
        }
		
		List<Vector3> verts;
		List<int> tris;
		List<Vector2> uvs;
        List<Vector2> crystal_uvs;
		List<Color> colors;
		List<Vector3> normals;
		
		PATile tile;
		PAPoint point;
		int x, y, li, i, cx, cy, cId, sx, sy;
		int w = settings.xCount, 
			h = settings.yCount; 
		int cw, ch;
		int chunkCountX, chunkCountY;
		float tileSize = settings.tileSize;
		GameObject go;
		string str;
        transform.position = new Vector3(-(float)settings.xCount / 2, 0, 0);
		Vector3 tpos = transform.position;
		PATileTerrainChunk chunk;
		
		Mesh mesh;
        MeshCollider meshCollider;
        //BoxCollider boxCollider;
		MeshFilter meshFilter;
		MeshRenderer meshRenderer;

        Mesh crystalMesh;
        MeshFilter crystalMeshFilter;
        MeshRenderer crystalMeshRenderer;
		
		if (w % settings.chunkSize > 0)	chunkCountX = w / settings.chunkSize + 1; 
		else chunkCountX = w / settings.chunkSize; 
		if (h % settings.chunkSize > 0) chunkCountY = h / settings.chunkSize + 1; 
		else chunkCountY = h / settings.chunkSize;
		
		settings.chunkCountX = chunkCountX;
		settings.chunkCountY = chunkCountY;
		
		settings.chunks = new PATileTerrainChunk[chunkCountX * chunkCountY];
		settings.tiles = new PATile[w * h];
		settings.points = new PAPoint[(w + 1) * (h + 1)];
        if(jsnode == null)
        {
            settings.tsTrans.Clear();
            settings.tsTypes.Clear();	
        }
		  
		for (cy = 0; cy < chunkCountY; ++cy)	
			for (cx = 0; cx < chunkCountX; ++cx) 
			{
				cId = chunkCountX * cy + cx;
				str = settings.name + "_chunk_" + cx + "x" + cy + "_" + cId;
				go = new GameObject(str);
				go.transform.parent = transform;
                go.layer = LayerMask.NameToLayer("TerrainChunk");
                GameObject crystalGo = new GameObject("crystal");
                crystalGo.transform.SetParent(go.transform);
                crystalGo.transform.localPosition = new Vector3(0f,0.1f,0f);
                crystalGo.SetActive(false);

                GameObject buildingsRootGo = new GameObject("buildingsRoot");
                buildingsRootGo.transform.SetParent(go.transform,false);
				//go.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
				//go.isStatic = true;
			 
				chunk = go.AddComponent<PATileTerrainChunk>();
				settings.chunks[cId] = chunk;
				chunk.hideFlags = /*HideFlags.HideInInspector | */HideFlags.NotEditable;
				chunk.settings.x = cx;
				chunk.settings.y = cy;
				chunk.settings.id = cId;
				
				//MeshFilter
				meshFilter = go.AddComponent<MeshFilter>();
                crystalMeshFilter = crystalGo.AddComponent<MeshFilter>();
				//meshFilter.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
			
				//Mesh
				mesh = new Mesh();
				mesh.name = str;
				mesh.Clear();

                crystalMesh = new Mesh();
                crystalMesh.name = "Crystal";

				//MeshCollider
                meshCollider = go.AddComponent<MeshCollider>();
                //boxCollider = go.AddComponent<BoxCollider>();
				//meshCollider.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
				
				//MeshRenderer
				meshRenderer = go.AddComponent<MeshRenderer>();
                crystalMeshRenderer = crystalGo.AddComponent<MeshRenderer>();
				//meshRenderer.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;

                go.AddComponent<NavMeshSourceTag>();

				//Creation of the mesh 
			
				//Chunk size
				if (cx == chunkCountX - 1) cw = settings.xCount - (chunkCountX - 1) * settings.chunkSize;	
				else cw = settings.chunkSize;
				if (cy == chunkCountY - 1) ch = settings.yCount - (chunkCountY - 1) * settings.chunkSize;	
				else ch = settings.chunkSize;
			
				go.transform.position = new Vector3(cx * settings.chunkSize * settings.tileSize, 0.0f, cy * settings.chunkSize * settings.tileSize) + tpos;
				
				verts = new List<Vector3>();
				uvs = new List<Vector2>();
                crystal_uvs = new List<Vector2>();
				colors = new List<Color>();
				tris = new List<int>();
				normals = new List<Vector3>();
				
				for (y = 0; y < ch; ++y)	
					for (x = 0; x < cw; ++x) 
					{
						//Tiles
						sx = cx * settings.chunkSize + x;
						sy = cy * settings.chunkSize + y;
						
						i = w * sy + sx;
						li = cw * y + x;
					
						settings.tiles[i] = new PATile();
						tile = settings.tiles[i];
						tile.name = i.ToString();
						tile.id = i;
						tile.chunkId = cId;
						tile.x = sx;
						tile.y = sy;
						tile.cx = x;
						tile.cy = y;
						tile.cId = cw * y + x;

                        if (jsnode == null)
                        {
                            tile.type = 0;
                            tile.bits = 0;
                        }
                        else
                        {
                            //JSONNode tileNode = jsnode["tiles"][tile.id.ToString()];
                            //tile.FromJson(tileNode);
                            tile.type = TerrainManager.defaultBrushType;
                            tile.bits = 0;
                        }
						
						//Center of tile
						tile.position = new Vector3(cx * settings.chunkSize * settings.tileSize, 0.0f, cy * settings.chunkSize * settings.tileSize) + 
										new Vector3(x * tileSize + tileSize / 2, 0.0f, y * tileSize + tileSize / 2);
				
						//vertices
						verts.Add(new Vector3(x * tileSize, 0.0f, y * tileSize));
						verts.Add(new Vector3(x * tileSize, 0.0f, y * tileSize + tileSize));
						verts.Add(new Vector3(x * tileSize + tileSize, 0.0f, y * tileSize + tileSize));
						verts.Add(new Vector3(x * tileSize + tileSize, 0.0f, y * tileSize));
						//uvs
                        if (jsnode == null)
                        {
                            uvs.Add(new Vector2(0.0f, 1.0f));
                            uvs.Add(new Vector2(1.0f, 1.0f));
                            uvs.Add(new Vector2(1.0f, 0.0f));
                            uvs.Add(new Vector2(0.0f, 0.0f));
                        }
                        else
                        {
                            PATSType type = settings.tsTypes[tile.type];
                            int index = type.GetRandomBaseIndex();
                            PATileUV uv = GetIndexUV(index);
                            uvs.Add(uv.p0);
                            uvs.Add(uv.p1);
                            uvs.Add(uv.p2);
                            uvs.Add(uv.p3);
                        }

                        crystal_uvs.Add(new Vector2(0.0f, 1.0f));
                        crystal_uvs.Add(new Vector2(1.0f, 1.0f));
                        crystal_uvs.Add(new Vector2(1.0f, 0.0f));
                        crystal_uvs.Add(new Vector2(0.0f, 0.0f));
                        
						//tris			
						tris.Add(li * 4 + 0);
						tris.Add(li * 4 + 1);
						tris.Add(li * 4 + 2);
						tris.Add(li * 4 + 0);
						tris.Add(li * 4 + 2);
						tris.Add(li * 4 + 3);
						//normals 
						normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
						normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
						normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
						normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
						//colors
						colors.Add(Color.white);
						colors.Add(Color.white);
						colors.Add(Color.white);
						colors.Add(Color.white);
					}				
								
				mesh.vertices = verts.ToArray();
				mesh.uv = uvs.ToArray();
				mesh.triangles = tris.ToArray();
				mesh.colors = colors.ToArray();	
				mesh.normals = normals.ToArray();
                
                crystalMesh.vertices = verts.ToArray();
                crystalMesh.uv = crystal_uvs.ToArray();
                crystalMesh.triangles = tris.ToArray();
			
				meshFilter.sharedMesh = mesh;
                meshCollider.sharedMesh = mesh;
				meshRenderer.enabled = true;
				meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				meshRenderer.receiveShadows = false;
				meshRenderer.sharedMaterial = settings.tilesetMaterial;

                crystalMeshFilter.sharedMesh = crystalMesh;
                crystalMeshRenderer.enabled = true;
                crystalMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                crystalMeshRenderer.receiveShadows = false;
                crystalMeshRenderer.sharedMaterial = Resources.Load<Material>("Terrain/Materials/tileset3");

				chunk.settings.mesh = mesh;
                chunk.settings.crystalGo = crystalGo;
                chunk.settings.buildingsRoot = buildingsRootGo.transform;
			}

        if (jsnode != null)
        {
            foreach (var crystal in settings.crystals)
            {
                PATile theTile = GetTile(crystal.id);
                PACrystalTile theCrystalTile = PACrystalTile.GetByTile(this,theTile);
                PATileTerrainChunk theChunk =  GetChunk(theTile.chunkId);
    
                GameObject shuijingGo  = null;
                if (Application.isPlaying)
                    shuijingGo = PoolManager.Pools["Shuijing"].Spawn(crystal.prefabName).gameObject;
                else
                    shuijingGo = Object.Instantiate(Resources.Load<GameObject>("Terrain\\Shuijing\\" + crystal.prefabName)) as GameObject;

                shuijingGo.transform.SetParent(theChunk.settings.crystalGo.transform);
                shuijingGo.transform.position = theCrystalTile.GetShuijingPos(this);
                Shuijing shuijing = shuijingGo.GetComponent<Shuijing>();
                shuijing.level = crystal.level;
                shuijing.elementType = crystal.elementType;
                theTile.shuijing = shuijing;
                shuijing.tile = theTile;
                crystal.shuijing = shuijing;
                shuijing.CreateBuildings(this);
            }
        }
		
		for (y = 0; y <= h; ++y)	
			for (x = 0; x <= w; ++x) 
			{
				//Prepare helper points for editor			
				i = (w + 1) * y + x;
				settings.points[i] = new PAPoint();
				point = settings.points[i];
				PrepareHelperPoint(point, x, y, w, h);		
			}
		
		SetTileSet(settings.tilesetMaterial);
		
		settings.created = true;	
		settings.finalized = false;
		//gameObject.isStatic = true; //Not for InGame use
        //tile.type = tileNode["type"].AsInt;
        //tile.toType = tileNode["toType"].AsInt;
        //tile.bits = tileNode["bits"].AsByte;
        //tile.tilesetIndex = tileNode["tilesetIndex"].AsInt;
        TerrainManager.instance.RepaintAllCrystals();
        //LocalNavMeshBuilder.instance.UpdateNavMesh();
		UpdateMesh();
	}
	
	public void DestroyTerrain()
	{
		if (settings.chunks != null)
		foreach (PATileTerrainChunk c in settings.chunks)
            GameObject.DestroyImmediate(c.gameObject);
		settings.chunks = null;
		
		//GameObject.DestroyImmediate(gameObject.GetComponent<MeshCollider>());
		//GameObject.DestroyImmediate(gameObject.GetComponent<MeshFilter>());
		//GameObject.DestroyImmediate(gameObject.GetComponent<MeshRenderer>());
		settings.created = false;
		settings.finalized = false;
		
		settings.tiles = null;
		settings.points = null;
		settings.tsTrans.Clear();
		settings.tsTypes.Clear();
	}	
	
	public void FinalizeTerrain()	
	{
		settings.finalized = true;
		settings.points = null;	
	}
	
	public bool GetWalkability(int x, int y) { return settings.tiles[settings.xCount * y + x].walkability; }
	public bool GetWalkability(PATile tile) { return tile.walkability; }
	
	public bool LoadHeightMap(Texture2D tex, float min, float max)
	{
		if (tex == null) return false;
		
		float tw = tex.width, 
			  th = tex.height;
		float two = tw / settings.xCount, 
		      tho = th / settings.yCount; 
		float d, ch, h;
		int i, j, ii, jj, pi, mi;
		
		List<Mesh> ms = new List<Mesh>();
		List<Vector3[]> vs = new List<Vector3[]>();
		Mesh mesh;
		Vector3[] vertices;
		bool c;
		PATile tile;
		PAPoint point;

		Color clr = Color.black;
		
		if (min < max) d = max - min; else d = min - max;
		
		for (j = 0; j <= settings.yCount; ++j)	
			for (i = 0; i <= settings.xCount; ++i) 
			{
				if (i == settings.xCount) ii = i - 1; else ii = i;
				if (j == settings.yCount) jj = j - 1; else jj = j;
				
				clr = tex.GetPixelBilinear((ii * two) / tw, (jj * tho) / th);
				ch = (clr.r + clr.g + clr.b) / 3.0f;
				
				h = min + d * ch;
			
				//SetPointHeight(i, j, h, false);
				point = settings.points[(settings.xCount + 1) * j + i];
				for (pi = 0; pi < 4; ++pi)
				if (point.t[pi] >= 0)
				{
					c = false;
					tile = GetTile(point.t[pi]);
					mesh = GetChunkMesh(tile.chunkId);
					vertices = null;
					for (mi = 0; mi < ms.Count; ++mi) if (ms[mi] == mesh) { c = true; vertices = vs[mi]; }
					if (vertices == null) vertices = mesh.vertices;
				
					vertices[tile.cId * 4 + point.p[pi]].y = Mathf.Clamp(h, settings.minHeight, settings.maxHeight);
					
					if (!c) { ms.Add(mesh); vs.Add(vertices); }
				}	
				
			}
		for (i = 0; i < ms.Count; ++i) ms[i].vertices = vs[i];
			
		UpdateMesh();
		return true;
	}
	
	public void SetTileSet(Material mat)
	{
		settings.tilesetMaterial = mat;
		foreach (PATileTerrainChunk c in settings.chunks)
		{
			c.GetComponent<Renderer>().sharedMaterial = mat;
		}
	}
	
	public PATileTerrain IsTerrain(Transform t)
	{ 
		foreach (PATileTerrainChunk c in settings.chunks) if (c.transform == t) return this;
		return null;
	}
	
	public void RecalcTilesetSizes()
	{
		settings.tilesetWidth = 1.0f / (float)settings.tilesetX;
		settings.tilesetHeight = 1.0f / (float)settings.tilesetY;
		settings.tilesetCount = settings.tilesetX * settings.tilesetY; 
	}
	
	public void AddNewTransition()
	{
		settings.tsTrans.Add(new PATSTransition());
	}
	
	protected PATSTransition FindTransition(int t1, int t2) 
	{ int id; return FindTransition(t1, t2, out id); }
	protected PATSTransition FindTransition(int t1, int t2, out int id)
	{
		int i;
		for (i = 0; i < settings.tsTrans.Count; ++i)
			if ((settings.tsTrans[i].from == t1 && settings.tsTrans[i].to == t2) ||
			    (settings.tsTrans[i].from == t2 && settings.tsTrans[i].to == t1)) 
		{ 
			id = i; 
			return settings.tsTrans[i]; 
		}
				
		id = -1;
		return null;
	}
	
	protected PATSTransition FindIntermediateTransition(int t1, int t2)	
	{ int id; return FindIntermediateTransition(t1, t2, out id); }
	protected PATSTransition FindIntermediateTransition(int t1, int t2, out int id)
	{
		int i, j, nt;
		for (i = 0; i < settings.tsTrans.Count; ++i)
		{
			if (settings.tsTrans[i].from == t1 || settings.tsTrans[i].to == t1) 
			{
				if (settings.tsTrans[i].from == t1) nt = settings.tsTrans[i].to;
				else nt = settings.tsTrans[i].from;
					
				for (j = 0; j < settings.tsTrans.Count; ++j)
					if (j != i)
					{
						if (settings.tsTrans[j].from == nt || settings.tsTrans[j].to == nt) 
						{							
							id = j;
							return settings.tsTrans[i];
						}
					}
			}
		}
		id = -1;
		return null;
	}
	
	protected PATSTransition FindTransitionForType(int t, out int id)
	{
		for (int i = 0; i < settings.tsTrans.Count; ++i)
			if (settings.tsTrans[i].from == t || settings.tsTrans[i].to == t) { id = i; return settings.tsTrans[i]; }
		id = -1;
		return null;
	}
	
	protected int FindTransitionId(int t)
	{
		for (int i = 0; i < settings.tsTrans.Count; ++i)
			if (settings.tsTrans[i].from == t || settings.tsTrans[i].to == t) return i;
		return -1;
	}
	
	protected int FindTransitionBitsId(byte bits)
	{
		for (int i = 0 ; i < 14; ++i) if (TRANSITION_BITS[i] == bits) return i;
		return -1;
	}
	
	public void AddNewType()
	{
		bool founded = false;
		int freeId = 0;
		do 
		{
			founded = false;
			foreach (PATSType t in settings.tsTypes)
			{
				if (freeId == t.id) { founded = true; break; }
			}
			
			if (!founded) break;
			freeId++;
		} while (true);
		
		
		PATSType nt = new PATSType();
		nt.id = freeId;
		nt.name = freeId.ToString();
		settings.tsTypes.Add(nt);
	}
	
	public void RemoveType(int index)
	{
		int tId = settings.tsTypes[index].id;
		foreach (PATSTransition t in settings.tsTrans)
		{
			if (t.from == tId) t.from = -1;
			if (t.to == tId) t.to = -1;
		}
		settings.tsTypes.RemoveAt(index);
	}
	
	public void RemoveTransition(int index)
	{
		settings.tsTrans.RemoveAt(index);
	}
	
	public void HideInUnity()
	{
        return;
		if (!settings.created) return;
		GameObject go;
		foreach (PATileTerrainChunk c in settings.chunks)
		{
			c.hideFlags = HideFlags.NotEditable;
			
			go = c.gameObject;
			go.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
			
			go.GetComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
			go.GetComponent<MeshCollider>().hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
			go.GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
		}
	}
	
	protected void PrepareHelperPoint(PAPoint point, int x, int y, int w, int h)
	{
		for (int k = 0; k < 4; ++k) { point.t[k] = -1; point.p[k] = -1; }			
			
		if (x == 0) // only RU and RD
		{
			if (y == 0) //only RU
			{ 
				point.t[0] = 0; point.p[0] = 0; 
			} 
			else if (y == settings.yCount) //only RD
			{
				point.t[0] = w * (y - 1); point.p[0] = 1; 											
			} else //RU and RD
			{
				point.t[0] = w * y;       point.p[0] = 0; //RU
				point.t[1] = w * (y - 1); point.p[1] = 1; //RD
			}
		} else if (x == settings.xCount) //only LU and LD
		{
			if (y == 0) //only LU
			{ 
				point.t[0] = x - 1; point.p[0] = 3; 
			} 
			else if (y == settings.yCount) //only LD
			{ 
				point.t[0] = w * (y - 1) + x - 1; point.p[0] = 2; 
			} 
			else //LU and LD
			{ 
				point.t[0] = w * y + x - 1; 	  point.p[0] = 3; //LU
				point.t[1] = w * (y - 1) + x - 1; point.p[1] = 2; //LD
			}
		} else //if (x > 0 && x < settings.xCount)
		{
			if (y == 0) //only LU and RU
			{  
				point.t[0] = x - 1; point.p[0] = 3; //LU
				point.t[1] = x;     point.p[1] = 0; //RU
			} 
			else if (y == settings.yCount) //only LD and RD
			{ 
				point.t[0] = w * (y - 1) + x - 1; point.p[0] = 2; //LD  
				point.t[1] = w * (y - 1) + x; 	  point.p[1] = 1; //RD
			} 
			else //LU and RU and LD and RD
			{ 
				point.t[0] = w * y + x - 1; 	  point.p[0] = 3; //LU
				point.t[1] = w * y + x;           point.p[1] = 0; //RU
				point.t[2] = w * (y - 1) + x - 1; point.p[2] = 2; //LD
				point.t[3] = w * (y - 1) + x;     point.p[3] = 1; //RD
			}
		}
	}
	
	//Helpers
	protected static float LinearFalloff(float distance, float radius) 
	{ return Mathf.Clamp01(1.0f - distance / radius); }
	
	protected static float GaussFalloff(float distance, float radius)
	{ return Mathf.Clamp01 (Mathf.Pow (360.0f, -Mathf.Pow (distance / radius, 2.5f) - 0.01f)); }
	
	protected bool IsOnSameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
	{
		Vector3 bma = b - a;
		Vector3 cp1 = Vector3.Cross(bma, (p1 - a));
		Vector3 cp2 = Vector3.Cross(bma, (p2 - a));
		return (Vector3.Dot(cp1, cp2) >= 0.0f); 
	}
	protected static byte InvertBits(byte bits) { byte b = 240; b |= bits; return (byte)~b; }
	protected static bool IsBitSet(byte testBits, byte b) { return ((testBits & (byte)(1 << b)) == 1); }

	//Properties
	public int width { get { return settings.xCount; } }
	public int height { get { return settings.yCount; } }
	public float tileSize { get { return settings.tileSize; } }	
	public PATile[] tiles { get { return settings.tiles; } }
	public PAPoint[] points { get { return settings.points; } }
	public bool isCreated { get { return settings.created; } }
}