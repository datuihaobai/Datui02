using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public partial class PATileTerrain
{
    [System.Serializable]
	public class EditorSettings
    {
        public int x = 32;
		public int y = 32;
		public float tileSize = 1.0f;
		public float maxHeight = 50.0f, minHeight = -50.0f;
		public Material tileSetMaterial;
		
		public int tilesetX = 1;
		public int tilesetY = 1;
		
		public int chunkSize = 8;
		public string name = "Terrain";
		
    }
    public EditorSettings editorSettings = new EditorSettings();
	
    /// <summary>
    /// 水晶格子
    /// 四个小格子组成一个水晶格子
    /// </summary>
    [System.Serializable]
    public class PACrystalTile
    {
        //水晶格子内部左下角的小格子
        public PATile leftBottomTile = null;

        public Vector3 GetShuijingPos(PATileTerrain tileTerrain)
        {
            if (leftBottomTile == null)
                return Vector3.zero;
            Vector3 pos = new Vector3(leftBottomTile.position.x + tileTerrain.settings.tileSize / 2, 0, leftBottomTile.position.z + tileTerrain.settings.tileSize / 2);
            return tileTerrain.transform.TransformPoint(pos);
        }

        public PATile[] GetOtherTiles(PATileTerrain tileTerrain)
        {
            if (leftBottomTile == null)
                return null;
            PATile[] otherTiles = new PATile[3];
            otherTiles[0] = tileTerrain.GetTile(leftBottomTile.x,leftBottomTile.y+1);
            otherTiles[1] = tileTerrain.GetTile(leftBottomTile.x+1, leftBottomTile.y + 1);
            otherTiles[2] = tileTerrain.GetTile(leftBottomTile.x+1, leftBottomTile.y);
            return otherTiles;
        }

        public static PACrystalTile GetByTile(PATileTerrain tileTerrain,PATile tile)
        {
            PACrystalTile crystalTile = new PACrystalTile();
            int x = tile.x / 2;
            int y = tile.y / 2;
            crystalTile.leftBottomTile = tileTerrain.GetTile(x * 2,y * 2);
            return crystalTile;
        }
    }

    public enum TileElementType
    {
        None = 0,
        Fire = 1,
        Wood = 2,
    }

    public class TileElementValue
    {
        public float value;

        public void ResetValue()
        {
            value = 0f;
        }

        public void AddValue(float addValue)
        {
            value += addValue;
        }

        public int GetIntValue()
        {
            return Mathf.CeilToInt(value);
        }
    }

    [System.Serializable]
    public class PATileElement
    {
        private Dictionary<TileElementType, TileElementValue> elementsDic = new Dictionary<TileElementType, TileElementValue>();

        public PATileElement()
        {
            elementsDic[TileElementType.Fire] = new TileElementValue();
            elementsDic[TileElementType.Wood] = new TileElementValue();
            Reset();
        }

        public int GetElementValue(TileElementType elementType)
        {
            return elementsDic[elementType].GetIntValue();
        }

        public void Reset()
        {
            foreach(var element in elementsDic.Values)
                element.ResetValue();
        }

        public void AddElement(TileElementType elementType,float addValue)
        {
            elementsDic[elementType].AddValue(addValue);
        }

        public int GetMaxElementValue()
        {
            TileElementType maxElementType = GetMaxElementType();
            return elementsDic[maxElementType].GetIntValue();
        }

        TileElementType GetMaxElementType()
        {
            TileElementType maxElementType = TileElementType.Fire;
            foreach(var elementType in elementsDic.Keys)
            {
                if (elementsDic[elementType].GetIntValue() >= elementsDic[maxElementType].GetIntValue())
                    maxElementType = elementType;
            }
            return maxElementType;
        }

        int GetBrushFromConfig(TileElementType elementType,int value)
        {
            foreach (var config in ConfigDataBase.instance.TileBrushConfigAsset.configs)
            {
                if (config.elementType == (int)elementType && config.level == value)
                    return config.brush;  
            }
            return -1;
        }

        //根据属性值返回地表贴图
        public int GetPaintBrushType()
        {
            TileElementType maxElementType = GetMaxElementType();
            return GetBrushFromConfig(maxElementType, elementsDic[maxElementType].GetIntValue());
        }
    }

	[System.Serializable]
	public class PATile
	{	
		public int id = -1; //tile id
		public int chunkId = -1; //[read only] chunk id
		public int x = -1; //[read only] X 
		public int y = -1; //[read only] Y
		public int cx = -1; //[read only] X in the chunk
		public int cy = -1; //[read only] Y in the chunk
		public int cId = -1;
		public string name; //tile name
		
		public Vector3 position; //helper position
		
		public int type = -1;
		public int toType = -1;
		public byte bits = 0;
        public int tilesetIndex = -1;

        //Game data
		public bool walkability = true; //for PathFinder
		public Object customData; //User data, you can assign your object
        public Shuijing shuijing = null;//tile内放置的水晶
        public PATileElement element = new PATileElement();

        public float distance = -1f;//距离水晶中心的距离
        public int decalTilesetIndex = -1;//贴花地格图index

        //public JSONNode ToJson()
        //{
        //    JSONNode jsnode = new JSONClass();

        //    int int32Data = ToInt32Data();
        //    jsnode["data"] = int32Data.ToString();

        //    return jsnode;
        //}

        //int ToInt32Data()
        //{
        //    byte[] byteDatas = new byte[4];
        //    byteDatas[0] = (byte)(type & 0xFF);
        //    byteDatas[1] = (byte)(toType & 0xFF);
        //    byteDatas[2] = bits;
        //    byteDatas[3] = (byte)(tilesetIndex & 0xFF);

        //    int data = System.BitConverter.ToInt32(byteDatas,0);
        //    return data; 
        //}

        //void FromInt32Data(int data)
        //{
        //    byte[] byteDatas = System.BitConverter.GetBytes(data);
        //    type = byteDatas[0];
        //    toType = byteDatas[1];
        //    bits = byteDatas[2];
        //    tilesetIndex = byteDatas[3];
        //}

        //public void FromJson(JSONNode jsnode)
        //{
        //    int int32Data = jsnode["data"].AsInt;
        //    FromInt32Data(int32Data);
        //}
    }
	
	[System.Serializable]
	public class PAPointXY 
	{ 
		public int x;
		public int y; 
		public PAPointXY(int xx, int yy) { x = xx; y = yy; }
		public override bool Equals(object obj) { if (((PAPointXY)obj).x == this.x && ((PAPointXY)obj).y == this.y)  return true; return false; }
		public override int GetHashCode() { return this.GetHashCode(); }
		public static bool operator==(PAPointXY p1, PAPointXY p2) { return p1.Equals(p2); }
        public static bool operator!=(PAPointXY p1, PAPointXY p2) { return !(p1 == p2); }
		//public override string ToString() { return "("+x + ", " + y +")"; }
	}
	
	[System.Serializable]
	public class PAPoint
	{		
		public int[] t = new int[4]; //Tile index
		public int[] p = new int[4]; //Tile vertex
	}
	
	public struct PATileUV 
	{ 
		public Vector2 p0, p1, p2, p3; 
	}
	
	[System.Serializable]
	public class PATSType
	{
		public int id = -1;
		public string name = "";
		
		public List<int> baseIndexes = new List<int>();
		
		public PATSType() { AddBaseIndex(); }
		public void AddBaseIndex() { baseIndexes.Add(0); }
		public void AddBaseIndex(int i) { baseIndexes.Add(i); }
		public void RemoveBaseIndex(int i) { if (baseIndexes.Count <= 0) AddBaseIndex();	if (baseIndexes.Count <= 1) return; baseIndexes.RemoveAt(i); }
		
		//Editor helpers
		public bool show = true;
	
        public int GetRandomBaseIndex()
        {
            return baseIndexes[RandomManager.instance.Range(0, baseIndexes.Count)];   
        }

        public JSONNode ToJson()
        {
            JSONNode jsnode = new JSONClass();
            jsnode["id"] = id.ToString();
            jsnode["name"] = name;
            JSONNode indexArray = new JSONArray();
            foreach(var index in baseIndexes)
                indexArray.Add(index.ToString());
            jsnode["baseIndexes"] = indexArray;
            return jsnode;
        }

        public void FromJson(JSONNode jsnode)
        {
            id = jsnode["id"].AsInt;
            name = jsnode["name"];
            baseIndexes.Clear();
            foreach (var index in jsnode["baseIndexes"].Childs)
                baseIndexes.Add(index.AsInt);
        }
    }
	
	[System.Serializable]
	public class PATSTransition //example: dirt(transition) <-> grass(type)
	{
		public int from = -1;
		public int to = -1;
		public string name = "";
		
		public int[] transitions = new int[14];
		
		public PATSTransition() { for (int i = 0; i < 14; ++i) transitions[i] = 0; }
		
		//Editor helpers
		public bool show = true;

        public JSONNode ToJson()
        {
            JSONNode jsnode = new JSONClass();
            jsnode["from"] = from.ToString();
            jsnode["to"] = to.ToString();
            jsnode["name"] = name.ToString();
            JSONNode transNodes = new JSONArray();
            foreach(var transition in transitions)
                transNodes.Add(transition.ToString());
            jsnode["transitions"] = transNodes;

            return jsnode;
        }

        public void FromJson(JSONNode jsnode)
        {
            from = jsnode["from"].AsInt;
            to = jsnode["to"].AsInt;
            name = jsnode["name"];
            int index = 0;
            foreach (var transition in jsnode["transitions"].Childs)
                transitions[index++] = transition.AsInt;
        }
	}

    [System.Serializable]
	public class PACrystal
    {
        public int id;
        public int level;
        public TileElementType elementType;
        public string prefabName;
        public int randomSeed;
        public Shuijing shuijing = null;

        public PACrystal()
        {
        }

        public PACrystal(int id ,int level,TileElementType elementType, string prefabName,int randomSeed)
        {
            this.id = id;
            this.level = level;
            this.elementType = elementType;
            this.prefabName = prefabName;
            this.randomSeed = randomSeed;
        }

        public JSONNode ToJson()
        {
            JSONNode jsnode = new JSONClass();
            jsnode["id"] = id.ToString();
            jsnode["level"] = level.ToString();
            jsnode["elementType"] = ((int)elementType).ToString();
            jsnode["prefabName"] = prefabName;
            jsnode["randomSeed"] = randomSeed.ToString();
            return jsnode;
        }

        public void FromJson(JSONNode jsnode)
        {
            id = jsnode["id"].AsInt;
            level = jsnode["level"].AsInt;
            elementType = (TileElementType)(jsnode["elementType"].AsInt);
            prefabName = jsnode["prefabName"];
            randomSeed = jsnode["randomSeed"].AsInt;
        }
    }
	
	[System.Serializable]
	public class PathData
    {
		public PATile[] data = null; //path tiles
		public int length = 0; //path length
		public bool found = false; //founded?
	}
	
	[System.Serializable]
    public class Settings
    {
		public bool created = false;
		public bool finalized = false;
		
		public PATileTerrainChunk[] chunks = null; //chunks
		public PATile[] tiles = null; //array of all tiles
		public PAPoint[] points = null; //helper point for edit the terrain
		public int xCount, yCount; //number of tiles along the X and Y (X and Z in the Unity3d space)
		public int chunkCountX, chunkCountY; //number of chunks along the X and Y
		public int chunkSize; //size of a one chunk
		public float tileSize; //size of a tile
		public Vector2 uvOffset; //uv offset for edit and painting
		public float maxHeight, minHeight; //max and min allowed height
		public float diagonalLength; 
		
		//For editor
		//TileSet
		public Material tilesetMaterial;
		public int tilesetX;
		public int tilesetY;
		public int tilesetCount;
		public float tilesetWidth;
		public float tilesetHeight;
		
		public string name;
		
		public List<PATSTransition> tsTrans = new List<PATSTransition>();
		public List<PATSType> tsTypes = new List<PATSType>();

        public List<PACrystal> crystals = new List<PACrystal>();

        public void RemoveCrystal(int id)
        {
            foreach(var crystal in crystals)
            {
                if (crystal.id == id)
                {
                    crystal.shuijing = null;
                    crystals.Remove(crystal);
                    return;
                }
            }
        }

        public void ClearCrystal()
        {
            foreach (var crystal in crystals)
                crystal.shuijing = null;
            crystals.Clear();
        }

        public PACrystal GetCrystal(int id)
        {
            foreach (var crystal in crystals)
            {
                if (crystal.id == id)
                {
                    return crystal;
                }
            }
            return null;
        }

        public JSONNode ToJson()
        {
            JSONNode jsnode = new JSONClass();
            jsnode["name"] = name;
            jsnode["xCount"] = xCount.ToString();
            jsnode["yCount"] = yCount.ToString();
            jsnode["chunkSize"] = chunkSize.ToString();
            jsnode["tileSize"] = tileSize.ToString();
            jsnode["uvOffset.x"] = uvOffset.x.ToString();
            jsnode["uvOffset.y"] = uvOffset.y.ToString();
            jsnode["tilesetX"] = tilesetX.ToString();
            jsnode["tilesetY"] = tilesetY.ToString();
            jsnode["tilesetMaterial"] = tilesetMaterial.name;
            
            //JSONNode tilesNodeDic = new JSONClass();
            //for (int i = 0; i < tiles.Length; i++)
            //{
            //    PATile tile = tiles[i];
            //    tilesNodeDic.Add(tile.id.ToString(), tile.ToJson());
            //}
            //jsnode["tiles"] = tilesNodeDic;
            
            JSONNode transNodeArray = new JSONArray();
            foreach (var tran in tsTrans)
                transNodeArray.Add(tran.ToJson());
            jsnode["tsTrans"] = transNodeArray;

            JSONNode typesNodeArray = new JSONArray();
            foreach (var type in tsTypes)
                typesNodeArray.Add(type.ToJson());
            jsnode["tsTypes"] = typesNodeArray;

            JSONNode crystalsNodeArray = new JSONArray();
            foreach (var crystal in crystals)
                crystalsNodeArray.Add(crystal.ToJson());
            jsnode["crystals"] = crystalsNodeArray;

            return jsnode;
        }

        public void FromJson(JSONNode jsnode)
        {
            name = jsnode["name"];
            xCount = jsnode["xCount"].AsInt;
            yCount = jsnode["yCount"].AsInt;
            chunkSize = jsnode["chunkSize"].AsInt;
            tileSize = jsnode["tileSize"].AsInt;
            uvOffset.x = jsnode["uvOffset.x"].AsFloat;
            uvOffset.y = jsnode["uvOffset.y"].AsFloat;
            tilesetX = jsnode["tilesetX"].AsInt;
            tilesetY = jsnode["tilesetY"].AsInt;
            tilesetMaterial = Resources.Load<Material>("Terrain/Materials/" + jsnode["tilesetMaterial"]);

            tsTrans.Clear();
            foreach (var tranNode in jsnode["tsTrans"].Childs)
            {
                PATSTransition tran = new PATSTransition();
                tran.FromJson(tranNode);
                tsTrans.Add(tran);
            }

            tsTypes.Clear();
            foreach (var typeNode in jsnode["tsTypes"].Childs)
            {
                PATSType type = new PATSType();
                type.FromJson(typeNode);
                tsTypes.Add(type);
            }

            crystals.Clear();
            foreach (var crystalNode in jsnode["crystals"].Childs)
            {
                PACrystal crystal = new PACrystal();
                crystal.FromJson(crystalNode);
                crystals.Add(crystal);
            }
        }
	}

    public Settings settings = new Settings();
	
	protected class IntermediateInfo
	{
		public int fromType, toType;
		public int imFromType, imToType;
	}

    private int[] TRANSITION_BITS = 
    {
        8,  //0  - 1000 
        4,  //1  - 0100 
        2,  //2  - 0010 
        1,  //3  - 0001 
        12, //4  - 1100 
        3,  //5  - 0011 
        13, //6  - 1101 
        14, //7  - 1110
        7,  //8  - 0111 
        11, //9  - 1011 
        5,  //10 - 0101 
        10, //11 - 1010
        9,  //12 - 1001 
        6   //13 - 0110
    }; 
}
