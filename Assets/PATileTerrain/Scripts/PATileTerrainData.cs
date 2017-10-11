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
    public class PABuildingTile
    {
        //水晶格子内部左下角的小格子
        public PATile keyTile = null;

        public Vector3 GetBuildingPos(PATileTerrain tileTerrain)
        {
            if (keyTile == null)
                return Vector3.zero;
            Vector3 pos = new Vector3(keyTile.position.x + tileTerrain.settings.tileSize / 2, 0, keyTile.position.z + tileTerrain.settings.tileSize / 2);
            return tileTerrain.transform.TransformPoint(pos);
        }

        public PATile[] GetOtherTiles(PATileTerrain tileTerrain)
        {
            if (keyTile == null)
                return null;
            PATile[] otherTiles = new PATile[3];
            otherTiles[0] = tileTerrain.GetTile(keyTile.x,keyTile.y+1);
            otherTiles[1] = tileTerrain.GetTile(keyTile.x+1, keyTile.y + 1);
            otherTiles[2] = tileTerrain.GetTile(keyTile.x+1, keyTile.y);
            return otherTiles;
        }

        public static PABuildingTile GetByTile(PATileTerrain tileTerrain,PATile tile)
        {
            PABuildingTile buildingTile = new PABuildingTile();
            int x = tile.x / 2;
            int y = tile.y / 2;
            buildingTile.keyTile = tileTerrain.GetTile(x * 2,y * 2);
            return buildingTile;
        }
    }

    public enum TileDecalType
    {
        Decal_1=1,//占1个实心格子
        Decal_2=2,//占横向2个实心格子
        Decal_4=4,//占2*2=4个实心格子
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

    // 贴花适用的地表属性类型
    public enum DecalSuitTileType
    {
        None = -1,//未知
        Base = 0,// 底色
        Fire = 1,// 火
        Wood = 2,// 木
        Sand = 3,// 火木融合
    }

    [System.Serializable]
    public class PATileElement
    {
        private Dictionary<TileElementType, TileElementValue> elementsDic = new Dictionary<TileElementType, TileElementValue>();

        public int FireValue
        {
            get { return GetElementValue(TileElementType.Fire); }
        }

        public int WoodValue
        {
            get { return GetElementValue(TileElementType.Wood); }
        }

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

        public bool Equals(PATileElement other)
        {
            if (other.FireValue == FireValue && other.WoodValue == WoodValue)
                return true;
            return false;
        }

        public bool IsMultiElement()
        {
            bool result = FireValue > 0 && WoodValue > 0;
            //if (Mathf.Abs(FireValue - WoodValue) >
            //    TerrainManager.instance.GetMinIgnoreElementValue())
            //    result = false;
            return result;
        }

        public bool IsEqualElement()
        {
            return FireValue == WoodValue;
        }

        public bool IsMultiAndEqualElement()
        {
            return IsMultiElement() && IsEqualElement();
        }

        bool IsBaseElement()
        {
            return FireValue == 0 && WoodValue == 0;
        }

        public bool IsSingleElement()
        {
            if (IsBaseElement())
                return false;
            return !IsMultiElement();
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

        public TileElementType GetMaxElementType()
        {
            TileElementType maxElementType = TileElementType.Fire;
            foreach(var elementType in elementsDic.Keys)
            {
                if (elementsDic[elementType].GetIntValue() >= elementsDic[maxElementType].GetIntValue())
                    maxElementType = elementType;
            }
            return maxElementType;
        }

        public int GetBrushFromConfig(TileElementType elementType, int value)
        {
            foreach (var config in ConfigDataBase.instance.TileBrushConfigAsset.configs)
            {
                if (config.elementType == (int)elementType && config.level == value)
                    return config.brush;  
            }
            return -1;
        }

        //根据属性值返回地表贴图
        public int GetMaxElementPaintBrushType()
        {
            TileElementType maxElementType = GetMaxElementType();
            return GetBrushFromConfig(maxElementType, elementsDic[maxElementType].GetIntValue());
        }

        public int GetSingleElementPaintBrushType(TileElementType elementType)
        {
            return GetBrushFromConfig(elementType, elementsDic[elementType].GetIntValue());
        }

        public DecalSuitTileType GetDecalSuitTileType()
        {
            if (IsMultiElement())
                return DecalSuitTileType.Sand;
            else if (FireValue > 0 && WoodValue == 0)
                return DecalSuitTileType.Fire;
            else if(WoodValue > 0 && FireValue == 0)
                return DecalSuitTileType.Wood;

            return DecalSuitTileType.None;
        }
    
        public TileElementType GetTileElementType()
        {
            if (FireValue - WoodValue > TerrainManager.instance.GetMinIgnoreElementValue())
                return TileElementType.Fire;
            else if (WoodValue - FireValue > TerrainManager.instance.GetMinIgnoreElementValue())
                return TileElementType.Wood;
            else if (FireValue > 0)
                return TileElementType.Fire;
            else if (WoodValue > 0)
                return TileElementType.Wood;
            else
                return TileElementType.None;
        }

        public static TileElementType GetTileElementType(PATile tile)
        {
            if (tile != null)
                tile.element.GetTileElementType();
            return TileElementType.None;
        }
    }

    //地格贴图类型
    public enum TileSetType
    {
        Full = 0,//实心
        Corner = 1,//角1:3
        BigCorner = 2,//大角3:1
        Edge = 3,//边

        //Mix = 4,//融合
    }

    // 四分之一tile的贴图属性枚举
    public enum QtrTileElementType
    {
        None = -1,//未知
        Base = 0,// 底色
        Fire = 1,// 火
        Wood = 2,// 木
        Sand = 3,// 火木融合
    
        Fire2 = 11,// 2级（深色）火
        Fire3 = 12,// 3级（红色）火

        Wood2 = 21,// 2级木
        Wood3 = 22,// 3级木
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
        public int specifiedIndex = -1;
        public UVRotateType rotateType = UVRotateType.None;
        //Game data
		public bool walkability = true; //for PathFinder
		public Object customData; //User data, you can assign your object
        public Shuijing shuijing = null;//tile内放置的水晶
        public PATileElement element = new PATileElement();

        public QtrTileElementType[] qtrTiles = null;//四分之一tile列表 顺序为leftBottom leftTop rightTop rightBottom

        public float distance = -1f;//距离水晶中心的距离
        public int decalTilesetIndex = -1;//贴花地格图index
        //public bool isFull;//true贴图全格，false贴图边角
        public TileSetType tileSetType;
        public Shuijing affectShuijing = null;//受影响的水晶，一个tile只保存第一个影响它的水晶

        public PATile()
        {
            qtrTiles = new QtrTileElementType[4];
            ResetQtrTiles();
        }
        //lb=leftBottom lt=leftTop rt=rightTop rb=rightBottom
        public void SetQtrTiles(QtrTileElementType lb,QtrTileElementType lt,QtrTileElementType rt,QtrTileElementType rb)
        {
            if (qtrTiles == null || qtrTiles.Length != 4)
            {
                Debug.LogError("qtrTiles == null || qtrTiles.Length != 4");
                return;
            }
            qtrTiles[0] = lb;
            qtrTiles[1] = lt;
            qtrTiles[2] = rt;
            qtrTiles[3] = rb;
        }

        // 四个qtrtile属性相同
        public bool IsElementFull()
        {
            if (qtrTiles[0] == qtrTiles[1] && qtrTiles[0] == qtrTiles[2] && qtrTiles[0] == qtrTiles[3])
                return true;
            return false;
        }

        public void SetQtrTiles(QtrTileElementType theSameType)
        {
            SetQtrTiles(theSameType, theSameType, theSameType, theSameType);
        }

        public void Reset()
        {
            ResetQtrTiles();
            specifiedIndex = -1;
            rotateType = UVRotateType.None;
            element.Reset();
            decalTilesetIndex = -1;
            distance = -1;
            affectShuijing = null;
        }

        void ResetQtrTiles()
        {
            if (qtrTiles == null)
                return;
            for (int i = 0; i < qtrTiles.Length; i++)
                qtrTiles[i] = QtrTileElementType.Base;
        }

        //是否完全设置了qtrTiles的属性
        public bool IsQtrTilesSet()
        {
            for (int i = 0; i < qtrTiles.Length; i++)
            {
                if (qtrTiles[i] == QtrTileElementType.None)
                    return false;
            }
            return true;
        }

        public void SetTileProp(int type, int toType, byte bits, int specifiedIndex = -1, UVRotateType rotateType = UVRotateType.None)
        {
            this.type = type;
            this.toType = toType;
            this.bits = bits;
            this.specifiedIndex = specifiedIndex;
            this.rotateType = rotateType;
        }

        // 存在几种元素的融合
        public int QtrTileTypeCount()
        {
            int count =0;
            if (HasQtrTileType(QtrTileElementType.Wood))
                count++;
            if (HasQtrTileType(QtrTileElementType.Fire))
                count++;
            if (HasQtrTileType(QtrTileElementType.Base))
                count++;
            if (HasQtrTileType(QtrTileElementType.Sand))
                count++;
            return count;
        }

        bool HasQtrTileType(QtrTileElementType theQtr)
        {
            foreach(var qtrTile in qtrTiles)
            {
                if (qtrTile == theQtr)
                    return true;
            }

            return false;
        }

        // 根据qtrtile推算bits
        byte GetBits(QtrTileElementType theQtr)
        {
            byte bits = 0;
            if (qtrTiles[0] == theQtr)
                bits += 8;
            if (qtrTiles[1] == theQtr)
                bits += 4;
            if (qtrTiles[2] == theQtr)
                bits += 2;
            if (qtrTiles[3] == theQtr)
                bits += 1;
            return bits;
        }

        //根据qtrtile（四分之一格子）计算绘制tile需要的属性。type，totype和bits ,
        //只适用于2种元素融合的情况，三种以上需要查表
        public void SetTilePropByQtrTile()
        {
            if(HasQtrTileType(QtrTileElementType.Fire2))
            {
                // fire2和fire融合
                if(HasQtrTileType(QtrTileElementType.Fire))
                {
                    type = FireLevel2Brush;
                    toType = FireLevel1Brush;
                    bits = GetBits(QtrTileElementType.Fire2);
                }
                // 纯fire2
                else
                {
                    type = FireLevel2Brush;
                    toType = FireLevel2Brush;
                    bits = 0;
                }
            }
            else if (HasQtrTileType(QtrTileElementType.Wood2))
            {
                // wood2和wood融合
                if (HasQtrTileType(QtrTileElementType.Wood))
                {
                    type = WoodLevel2Brush;
                    toType = WoodLevel1Brush;
                    bits = GetBits(QtrTileElementType.Wood2);
                }
                // 纯wood2
                else
                {
                    type = WoodLevel2Brush;
                    toType = WoodLevel2Brush;
                    bits = 0;
                }
            }
            else if (HasQtrTileType(QtrTileElementType.Wood))
            {
                // wood和fire融合
                if (HasQtrTileType(QtrTileElementType.Fire))
                {
                    type = WoodLevel1Brush;
                    toType = FireLevel1Brush;
                    bits = GetBits(QtrTileElementType.Wood);
                }
                // wood和base融合
                else if (HasQtrTileType(QtrTileElementType.Base))
                {
                    type = WoodLevel1Brush;
                    toType = BaseBrush;
                    bits = GetBits(QtrTileElementType.Wood);
                }
                else if (HasQtrTileType(QtrTileElementType.Sand))
                {
                    type = WoodLevel1Brush;
                    toType = SandBrush;
                    bits = GetBits(QtrTileElementType.Wood);
                }
                // 纯wood
                else
                {
                    type = WoodLevel1Brush;
                    toType = WoodLevel1Brush;
                    bits = 0;
                }
            }
            else if (HasQtrTileType(QtrTileElementType.Fire))
            {
                // fire和base融合
                if (HasQtrTileType(QtrTileElementType.Base))
                {
                    type = FireLevel1Brush;
                    toType = BaseBrush;
                    bits = GetBits(QtrTileElementType.Fire);
                }
                else if (HasQtrTileType(QtrTileElementType.Sand))
                {
                    type = FireLevel1Brush;
                    toType = SandBrush;
                    bits = GetBits(QtrTileElementType.Fire);
                }
                // 纯fire
                else
                {
                    type = FireLevel1Brush;
                    toType = FireLevel1Brush;
                    bits = 0;
                }
            }
            else if (HasQtrTileType(QtrTileElementType.Sand))
            {
                // sand和base融合
                if (HasQtrTileType(QtrTileElementType.Base))
                {
                    type = SandBrush;
                    toType = BaseBrush;
                    bits = GetBits(QtrTileElementType.Sand);
                }
                //纯sand
                else
                {
                    type = SandBrush;
                    toType = SandBrush;
                    bits = 0;
                }
            }
        }

        public int GetSingleElementPaintBrushType(TileElementType elementType)
        {
            if (element.IsSingleElement())
                return element.GetSingleElementPaintBrushType(elementType);
            else if (element.IsMultiElement())
            {
                if (elementType == TileElementType.Fire)
                    return FireLevel1Brush;
                else if (elementType == TileElementType.Wood)
                    return WoodLevel1Brush;
            }
            return -1;
        }

        public static QtrTileElementType GetQtrTileElementType(PATile tile, int qtrIndex)
        {
            if (tile == null)
                return QtrTileElementType.None;
            else
                return tile.qtrTiles[qtrIndex];
        }

        //将没有完美融合的qtrtile设置成none
        public bool ProcessMixPerfect(PATileTerrain tileTerrain)
        {
            PATile[] nTiles = tileTerrain.GetNeighboringTilesNxN(this, 1);

            PATile leftBottomTile = nTiles[0];
            PATile leftTile = nTiles[1];
            PATile leftTopTile = nTiles[2];
            PATile topTile = nTiles[3];
            PATile rightTopTile = nTiles[4];
            PATile rightTile = nTiles[5];
            PATile rightBottomTile = nTiles[6];
            PATile bottomTile = nTiles[7];
            bool isPerfect = true;

            QtrTileElementType qte0 = GetQtrTileElementType(bottomTile, 1);
            QtrTileElementType qte1 = GetQtrTileElementType(leftBottomTile, 2);
            QtrTileElementType qte2 = GetQtrTileElementType(leftTile, 3);
            QtrTileElementType qte = qtrTiles[0];
            if (qte == qte0 && qte == qte1 && qte == qte2)
            { }
            else
            {
                isPerfect = false;
                qtrTiles[0] = QtrTileElementType.None;
            }
               

            qte0 = GetQtrTileElementType(leftTile, 2);
            qte1 = GetQtrTileElementType(leftTopTile, 3);
            qte2 = GetQtrTileElementType(topTile, 0);
            qte = qtrTiles[1];
            if (qte == qte0 && qte == qte1 && qte == qte2)
            { }
            else
            {
                isPerfect = false;
                qtrTiles[1] = QtrTileElementType.None;
            }

            qte0 = GetQtrTileElementType(topTile, 3);
            qte1 = GetQtrTileElementType(rightTopTile, 0);
            qte2 = GetQtrTileElementType(rightTile, 1);
            qte = qtrTiles[2];
            if (qte == qte0 && qte == qte1 && qte == qte2)
            { }
            else
            {
                isPerfect = false;
                qtrTiles[2] = QtrTileElementType.None;
            }

            qte0 = GetQtrTileElementType(rightTile, 0);
            qte1 = GetQtrTileElementType(rightBottomTile, 1);
            qte2 = GetQtrTileElementType(bottomTile, 2);
            qte = qtrTiles[3];
            if (qte == qte0 && qte == qte1 && qte == qte2)
            { }
            else
            {
                isPerfect = false;
                qtrTiles[3] = QtrTileElementType.None;
            }

            return isPerfect;
        }

        public static bool IsSingleElement(PATile tile)
        {
            if (tile == null)
                return false;
            else
                return tile.element.IsSingleElement();
        }

        public static bool IsMultiElement(PATile tile)
        {
            if (tile == null)
                return false;
            else
                return tile.element.IsMultiElement();
        }

        public static bool IsTileSetType(PATile tile, TileSetType tst)
        {
            if (tile == null)
                return false;
            else
                return tile.tileSetType == tst;
        }
    
        public float Distance(PATile other)
        {
            return Vector2.Distance(new Vector2(x,y),new Vector2(other.x,other.y));
        }
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
        public int[] transitionCounts = new int[14];

		public PATSTransition() 
        {
            for (int i = 0; i < 14; ++i) 
                transitions[i] = 0;
            for (int i = 0; i < 14; ++i)
                transitionCounts[i] = 1; 
        }
		
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
            transNodes = new JSONArray();
            foreach (var transitionCount in transitionCounts)
                transNodes.Add(transitionCount.ToString());
            jsnode["transitionCounts"] = transNodes;

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
            index = 0;
            foreach (var transitionCount in jsnode["transitionCounts"].Childs)
                transitionCounts[index++] = transitionCount.AsInt;
        }
	}

    [System.Serializable]
    public class PABuilding
    {
        public int id;
        public TileElementType elementType;
        public string prefabName;
        public int belongShuijingId;

        public PABuilding()
        {
        }

        public PABuilding(int id, TileElementType elementType, string prefabName)
        {
            this.id = id;
            this.elementType = elementType;
            this.prefabName = prefabName;
        }

        public virtual JSONNode ToJson()
        {
            JSONNode jsnode = new JSONClass();
            jsnode["id"] = id.ToString();
            jsnode["elementType"] = ((int)elementType).ToString();
            jsnode["prefabName"] = prefabName;
            jsnode["belongShuijingId"] = belongShuijingId.ToString();
            return jsnode;
        }

        public virtual void FromJson(JSONNode jsnode)
        {
            id = jsnode["id"].AsInt;
            elementType = (TileElementType)(jsnode["elementType"].AsInt);
            prefabName = jsnode["prefabName"];
            belongShuijingId = jsnode["belongShuijingId"].AsInt;
        }
    }

    [System.Serializable]
	public class PACrystalBuilding : PABuilding
    {
        public int level;
        public int randomSeed;
        public Shuijing shuijing = null;
        public List<PABuilding> buildings = new List<PABuilding>();//属于水晶的功能建筑列表

        public PACrystalBuilding()
        {
        }

        public PACrystalBuilding(int id ,int level,TileElementType elementType, string prefabName,int randomSeed)
            : base(id,elementType,prefabName)
        {
            this.level = level;
            this.randomSeed = randomSeed;
        }

        public void AddBuilding(PABuilding building)
        {
            buildings.Add(building);
        }

        public void RemoveBuilding(PABuilding building)
        {
            buildings.Remove(building);
        }

        public void ClearBuildings()
        {
            buildings.Clear();
        }

        public override JSONNode ToJson()
        {
            JSONNode jsnode = base.ToJson();
            jsnode["level"] = level.ToString();
            jsnode["randomSeed"] = randomSeed.ToString();
            JSONNode buildingsNodeArray = new JSONArray();
            foreach (var building in buildings)
                buildingsNodeArray.Add(building.ToJson());
            jsnode["buildings"] = buildingsNodeArray;
            return jsnode;
        }

        public override void FromJson(JSONNode jsnode)
        {
            base.FromJson(jsnode);
            level = jsnode["level"].AsInt;
            randomSeed = jsnode["randomSeed"].AsInt;
            buildings.Clear();
            foreach (var buildingNode in jsnode["buildings"].Childs)
            {
                PABuilding building = new PABuilding();
                building.FromJson(buildingNode);
                buildings.Add(building);
            }
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

        public List<PACrystalBuilding> crystals = new List<PACrystalBuilding>();

        public void RemoveCrystal(int id)
        {
            foreach(var crystal in crystals)
            {
                if (crystal.id == id)
                {
                    crystal.ClearBuildings();
                    crystal.shuijing = null;
                    crystals.Remove(crystal);
                    return;
                }
            }
        }

        public void ClearCrystal()
        {
            foreach (var crystal in crystals)
            {
                crystal.ClearBuildings();
                crystal.shuijing = null;
            } 
            crystals.Clear();
        }

        public PACrystalBuilding GetCrystalBuilding(int id)
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
                PACrystalBuilding crystalBuilding = new PACrystalBuilding();
                crystalBuilding.FromJson(crystalNode);
                crystals.Add(crystalBuilding);
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
