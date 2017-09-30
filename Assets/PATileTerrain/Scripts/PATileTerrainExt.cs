using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public partial class PATileTerrain
{
    public const int FireLevel1Brush = 1;
    public const int WoodLevel1Brush = 4;
    public const int SandBrush = 7;

    public class TilePaintSample
    {
        public QtrTileElementType[] qtrTiles = null;
        public int type;
        public int toType;
        public byte bits;

        public TilePaintSample(QtrTileElementType lb, QtrTileElementType lt, QtrTileElementType rt, QtrTileElementType rb,
            int type , int toType, byte bits)
        {
            qtrTiles = new QtrTileElementType[4];
            qtrTiles[0] = lb;
            qtrTiles[1] = lt;
            qtrTiles[2] = rt;
            qtrTiles[3] = rb;
            this.type = type;
            this.toType = toType;
            this.bits = bits;
        }
    
        public bool IsFitTile(PATile tile)
        {
            if (tile.qtrTiles[0] == qtrTiles[0] && tile.qtrTiles[1] == qtrTiles[1] &&
                tile.qtrTiles[2] == qtrTiles[2] && tile.qtrTiles[3] == qtrTiles[3])
                return true;
            return false;
        }
    }

    public List<TilePaintSample> tilePaintSamples = new List<TilePaintSample>();

    public void GenerateTilePaintSamples()
    {
        TilePaintSample tps = new TilePaintSample(
            QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Sand, QtrTileElementType.Sand, 
            FireLevel1Brush, SandBrush, 12);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Wood, QtrTileElementType.Wood, 
            WoodLevel1Brush, SandBrush, 3);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Wood, QtrTileElementType.Sand, QtrTileElementType.Sand, 
            WoodLevel1Brush, SandBrush, 12);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Fire, QtrTileElementType.Fire,
            FireLevel1Brush, SandBrush, 3);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Wood, 
            WoodLevel1Brush, SandBrush, 1);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Fire, QtrTileElementType.Sand, QtrTileElementType.Sand,
            FireLevel1Brush, SandBrush, 4);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Sand,
            FireLevel1Brush, SandBrush, 6);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Wood,
            WoodLevel1Brush, SandBrush, 9);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Fire, QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Sand,
            FireLevel1Brush, SandBrush, 8);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Wood, QtrTileElementType.Sand,
            WoodLevel1Brush, SandBrush, 2);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Sand, QtrTileElementType.Wood, QtrTileElementType.Wood,
            WoodLevel1Brush, SandBrush, 11);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Sand,
            FireLevel1Brush, SandBrush, 14);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Fire,
            FireLevel1Brush, SandBrush, 7);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Wood, QtrTileElementType.Sand, QtrTileElementType.Wood,
            WoodLevel1Brush, SandBrush, 13);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Sand,
            WoodLevel1Brush, SandBrush, 8);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Wood, QtrTileElementType.Wood, QtrTileElementType.Sand,
            WoodLevel1Brush, SandBrush, 6);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Fire, QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Fire,
            FireLevel1Brush, SandBrush, 9);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Fire, QtrTileElementType.Sand, QtrTileElementType.Fire, QtrTileElementType.Fire,
            FireLevel1Brush, SandBrush, 11);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Wood, QtrTileElementType.Wood, QtrTileElementType.Sand,
            WoodLevel1Brush, SandBrush, 14);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Sand, QtrTileElementType.Wood, QtrTileElementType.Wood, QtrTileElementType.Wood,
            WoodLevel1Brush, SandBrush, 7);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Sand, QtrTileElementType.Fire,
            FireLevel1Brush, SandBrush, 13);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Wood, QtrTileElementType.Wood,
            WoodLevel1Brush,FireLevel1Brush, 3);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Wood,
            WoodLevel1Brush, FireLevel1Brush, 9);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Fire, QtrTileElementType.Fire, QtrTileElementType.Fire,
            WoodLevel1Brush, FireLevel1Brush, 8);
        tilePaintSamples.Add(tps);
        tps = new TilePaintSample(
            QtrTileElementType.Wood, QtrTileElementType.Wood, QtrTileElementType.Fire, QtrTileElementType.Wood,
            WoodLevel1Brush, FireLevel1Brush, 13);
        tilePaintSamples.Add(tps);
    }

    public TilePaintSample GetFitTilePaintSample(PATile tile)
    {
        foreach(var tps in tilePaintSamples)
            if (tps.IsFitTile(tile))
                return tps;
        return null;
    }

    /// <summary>
    /// 使用json格式将地图数据存储到文件中
    /// </summary>
    public void SaveTerrain()
    {
        string content = settings.ToJson().ToString();
        string path = Application.persistentDataPath + "/datui_terrain";
        Debug.Log(path);
        File.Delete(path);
        if (string.IsNullOrEmpty(content))
            return;
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        sw.Flush();
        sw.BaseStream.Seek(0, SeekOrigin.Begin);
        sw.Write(content);
        sw.Close();
    }

    /// <summary>
    /// 从json格式的文件中读取地图
    /// </summary>
    public void LoadTerrain()
    {
        string path = Application.persistentDataPath + "/datui_terrain";
        Debug.Log(path);
        if (File.Exists(path))
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            fs.Close();

            try
            {
                string content = System.Text.Encoding.UTF8.GetString(bytes);
                if (string.IsNullOrEmpty(content))
                    return;
                JSONNode jsnode = JSONNode.Parse(content);
                if (jsnode != null)
                    CreateTerrain(jsnode);
            }
            catch (System.Exception e)
            { }
        }
        else
        {
            StartCoroutine(LoadDefaultTerrain());
        }
    }

    IEnumerator LoadDefaultTerrain()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        string filepath = "file://" + Application.streamingAssetsPath + "/" + "datui_terrain";
#elif UNITY_IPHONE  
        string filepath = Application.dataPath +"/Raw"+"/datui_terrain";  
#elif UNITY_ANDROID  
        string filepath = "jar:file://" + Application.dataPath + "!/assets/"+"datui_terrain";  
#endif
       
        WWW data = new WWW(filepath);
        yield return data;

        if (string.IsNullOrEmpty(data.error))
        {
            string content = data.text;
            JSONNode jsnode = JSONNode.Parse(content);
            if (jsnode != null)
                CreateTerrain(jsnode);
        }
        else
            Debug.Log(data.error);
    }

    public void ResetTile()
    {
        foreach (var tile in tiles)
            tile.Reset();
    }

    public void ShowCrystal(bool isShow)
    {
        if (settings.chunks == null)
            return;

        foreach (PATileTerrainChunk c in settings.chunks)
        {
            c.settings.crystalGo.SetActive(isShow);
            c.settings.decoratesRoot.gameObject.SetActive(!isShow);
        }
    }

    void PaintTileDecal(PATile tile, UVRotateType rotateType = UVRotateType.None)
    {
        PaintNormalTile(tile,0,tile.decalTilesetIndex,rotateType);
    }

    void PaintNormalTile(PATile tile, int t, int specifiedIndex = -1, UVRotateType rotateType = UVRotateType.None)
    {
        if (tile == null)
            return;

        tile.SetTileProp(t,t,0,specifiedIndex,rotateType);
        UpdateTileUV(tile);
    }

    public enum TileElementState
    {
        None,
        TotalFire,// 完全火属性
        TotalWood,// 完全木属性
        //FireMax,//  火属性最大
        //WoodMax,// 木属性最大
        //FireWoodEqual,// 火木属性相等
        //Zero,// 无属性
    }

    TileElementState GetTileElementState(PATile tile)
    {
        TileElementState state = TileElementState.None;
        //if (tile.element.FireValue == tile.element.WoodValue && tile.element.FireValue == 0)
        //    state = TileElementState.Zero;
        //else 
        if (tile.element.FireValue > 0 && tile.element.WoodValue == 0)
            state = TileElementState.TotalFire;
        else if (tile.element.WoodValue > 0 && tile.element.FireValue == 0)
            state = TileElementState.TotalWood;
        //else if (tile.element.FireValue > tile.element.WoodValue)
        //    state = TileElementState.FireMax;
        //else if (tile.element.WoodValue > tile.element.FireValue)
        //    state = TileElementState.WoodMax;
        //else if (tile.element.WoodValue == tile.element.FireValue)
        //    state = TileElementState.FireWoodEqual;
        return state;
    }

    //通过周围8格计算当前格子该使用哪种属性进行融合
    QtrTileElementType GetQtrTileElementType(PATile[] nTiles)
    {
        //int index = 0;
        foreach (var tile in nTiles)
        {
            if (tile.element.IsSingleElement())
            {
                //if (index++ % 2 == 0)
                //    continue;

                TileElementState tileElementState = GetTileElementState(tile);
                if (tileElementState == TileElementState.TotalFire)
                    return QtrTileElementType.Fire;
                else if (tileElementState == TileElementState.TotalWood)
                    return QtrTileElementType.Wood;
            } 
        }
        return QtrTileElementType.Base;
    }

    // index=0=lb index=1=lt index=2=rt index=3=rb
    bool IsCornerMix(PATile[] nTiles,int index)
    {
        PATile leftBottomTile = nTiles[0];
        PATile leftTile = nTiles[1];
        PATile leftTopTile = nTiles[2];
        PATile topTile = nTiles[3];
        PATile rightTopTile = nTiles[4];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];

        if (index == 0)
        {
            if (leftBottomTile.element.IsSingleElement() &&
                leftTile.element.IsMultiElement() &&
                leftTopTile.element.IsMultiElement() &&
                topTile.element.IsMultiElement() &&
                rightTopTile.element.IsMultiElement() &&
                rightTile.element.IsMultiElement() &&
                rightBottomTile.element.IsMultiElement() &&
                bottomTile.element.IsMultiElement())
                return true;
        }
        else if(index == 1)
        {
            if (leftBottomTile.element.IsMultiElement() &&
                leftTile.element.IsMultiElement() &&
                leftTopTile.element.IsSingleElement() &&
                topTile.element.IsMultiElement() &&
                rightTopTile.element.IsMultiElement() &&
                rightTile.element.IsMultiElement() &&
                rightBottomTile.element.IsMultiElement() &&
                bottomTile.element.IsMultiElement())
                return true;
        }
        else if (index == 2)
        {
            if (leftBottomTile.element.IsMultiElement() &&
                leftTile.element.IsMultiElement() &&
                leftTopTile.element.IsMultiElement() &&
                topTile.element.IsMultiElement() &&
                rightTopTile.element.IsSingleElement() &&
                rightTile.element.IsMultiElement() &&
                rightBottomTile.element.IsMultiElement() &&
                bottomTile.element.IsMultiElement())
                return true;
        }
        else if (index == 3)
        {
            if (leftBottomTile.element.IsMultiElement() &&
                leftTile.element.IsMultiElement() &&
                leftTopTile.element.IsMultiElement() &&
                topTile.element.IsMultiElement() &&
                rightTopTile.element.IsMultiElement() &&
                rightTile.element.IsMultiElement() &&
                rightBottomTile.element.IsSingleElement() &&
                bottomTile.element.IsMultiElement())
                return true;
        }

        return false;
    }

    bool IsFullMix(PATile[] nTiles)
    {
        PATile leftBottomTile = nTiles[0];
        PATile leftTile = nTiles[1];
        PATile leftTopTile = nTiles[2];
        PATile topTile = nTiles[3];
        PATile rightTopTile = nTiles[4];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];

        if (leftBottomTile.element.IsMultiElement() &&
            leftTile.element.IsMultiElement() &&
            leftTopTile.element.IsMultiElement() &&
            topTile.element.IsMultiElement() &&
            rightTopTile.element.IsMultiElement() &&
            rightTile.element.IsMultiElement() &&
            rightBottomTile.element.IsMultiElement() &&
            bottomTile.element.IsMultiElement())
            return true;

        return false;
    }

    void PaintAMultiElementTile(PATile tile, List<PATile> postProcessMultiElementTiles = null)
    {
        if (tile.element.IsSingleElement())
            return;

        PATile[] nTiles = GetNeighboringTilesNxN(tile, 1);

        PATile leftBottomTile = nTiles[0];
        PATile leftTile = nTiles[1];
        PATile leftTopTile = nTiles[2];
        PATile topTile = nTiles[3];
        PATile rightTopTile = nTiles[4];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];
        
        int t = 0;
        //QtrTileElementType qtrTileElementType = QtrTileElementType.None;
        QtrTileElementType qtrTileElementType = GetQtrTileElementType(nTiles);
        if (qtrTileElementType == QtrTileElementType.Fire)
            t = FireLevel1Brush;
        else if (qtrTileElementType == QtrTileElementType.Wood)
            t = WoodLevel1Brush;

        TileElementState leftElementState = GetTileElementState(leftTile);
        TileElementState rightElementState = GetTileElementState(rightTile);
        TileElementState topElementState = GetTileElementState(topTile);
        TileElementState bottomElementState = GetTileElementState(bottomTile);
        TileElementState leftTopElementState = GetTileElementState(leftTopTile);
        TileElementState leftBottomElementState = GetTileElementState(leftBottomTile);
        TileElementState rightTopElementState = GetTileElementState(rightTopTile);
        TileElementState rightBottomElementState = GetTileElementState(rightBottomTile);

        if (leftTopTile.element.IsSingleElement() &&
            leftTile.element.IsSingleElement() &&
            topTile.element.IsSingleElement() &&
            rightBottomTile.element.IsMultiElement() &&
            leftTopTile.tileSetType == TileSetType.Full &&
            leftTile.tileSetType == TileSetType.Full &&
            topTile.tileSetType == TileSetType.Full)
        {
            if (leftTopElementState == TileElementState.TotalFire)
            {
                qtrTileElementType = QtrTileElementType.Fire;
                t = FireLevel1Brush;
            }
            else if (leftTopElementState == TileElementState.TotalWood)
            {
                qtrTileElementType = QtrTileElementType.Wood;
                t = WoodLevel1Brush;
            } 

            tile.SetTileProp(t, SandBrush, 14);
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, qtrTileElementType, QtrTileElementType.Sand);
            tile.tileSetType = TileSetType.BigCorner;
        }
        else if (rightTopTile.element.IsSingleElement() &&
            rightTile.element.IsSingleElement() &&
            topTile.element.IsSingleElement() &&
            leftBottomTile.element.IsMultiElement() &&
            rightTopTile.tileSetType == TileSetType.Full &&
            rightTile.tileSetType == TileSetType.Full &&
            topTile.tileSetType == TileSetType.Full)
        {
            if (rightTopElementState == TileElementState.TotalFire)
            {
                qtrTileElementType = QtrTileElementType.Fire;
                t = FireLevel1Brush;
            }
            else if (rightTopElementState == TileElementState.TotalWood)
            {
                qtrTileElementType = QtrTileElementType.Wood;
                t = WoodLevel1Brush;
            } 

            tile.SetTileProp(t, SandBrush, 7);
            tile.SetQtrTiles(QtrTileElementType.Sand, qtrTileElementType, qtrTileElementType, qtrTileElementType);
            tile.tileSetType = TileSetType.BigCorner;
        }
        else if (leftBottomTile.element.IsSingleElement() &&
            leftTile.element.IsSingleElement() &&
            bottomTile.element.IsSingleElement() &&
            rightTopTile.element.IsMultiElement() &&
            leftBottomTile.tileSetType == TileSetType.Full &&
            leftTile.tileSetType == TileSetType.Full &&
            bottomTile.tileSetType == TileSetType.Full)
        {

            if (leftBottomElementState == TileElementState.TotalFire)
            {
                qtrTileElementType = QtrTileElementType.Fire;
                t = FireLevel1Brush;
            }
            else if (leftBottomElementState == TileElementState.TotalWood)
            {
                qtrTileElementType = QtrTileElementType.Wood;
                t = WoodLevel1Brush;
            } 

            tile.SetTileProp(t, SandBrush, 13);
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, QtrTileElementType.Sand, qtrTileElementType);
            tile.tileSetType = TileSetType.BigCorner;
        }
        else if (rightBottomTile.element.IsSingleElement() &&
            rightTile.element.IsSingleElement() &&
            bottomTile.element.IsSingleElement() &&
            leftTopTile.element.IsMultiElement() &&
            rightBottomTile.tileSetType == TileSetType.Full &&
            rightTile.tileSetType == TileSetType.Full &&
            bottomTile.tileSetType == TileSetType.Full)
        {

            if (rightBottomElementState == TileElementState.TotalFire)
            {
                qtrTileElementType = QtrTileElementType.Fire;
                t = FireLevel1Brush;
            }
            else if (rightBottomElementState == TileElementState.TotalWood)
            {
                qtrTileElementType = QtrTileElementType.Wood;
                t = WoodLevel1Brush;
            } 

            tile.SetTileProp(t, SandBrush, 11);
            tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Sand, qtrTileElementType, qtrTileElementType);
            tile.tileSetType = TileSetType.BigCorner;
        }

        else if (IsFullMix(nTiles))
        {
            tile.SetTileProp(SandBrush, SandBrush, 0);
            tile.SetQtrTiles(QtrTileElementType.Sand);
            tile.tileSetType = TileSetType.Full;
        }

        else if (IsCornerMix(nTiles, 0))
        {
            tile.SetTileProp(t, SandBrush, 8);
            tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Sand);
            tile.tileSetType = TileSetType.Corner;
        }
        else if (IsCornerMix(nTiles, 1))
        {
            tile.SetTileProp(t, SandBrush, 4);
            tile.SetQtrTiles(QtrTileElementType.Sand, qtrTileElementType, QtrTileElementType.Sand, QtrTileElementType.Sand);
            tile.tileSetType = TileSetType.Corner;
        }
        else if (IsCornerMix(nTiles, 2))
        {
            tile.SetTileProp(t, SandBrush, 2);
            tile.SetQtrTiles(QtrTileElementType.Sand, QtrTileElementType.Sand, qtrTileElementType, QtrTileElementType.Sand);
            tile.tileSetType = TileSetType.Corner;
        }
        else if (IsCornerMix(nTiles, 3))
        {
            tile.SetTileProp(t, SandBrush, 1);
            tile.SetQtrTiles(QtrTileElementType.Sand, QtrTileElementType.Sand, QtrTileElementType.Sand, qtrTileElementType);
            tile.tileSetType = TileSetType.Corner;
        }

        else
        {
            postProcessMultiElementTiles.Add(tile);
            tile.SetQtrTiles(QtrTileElementType.None);
            return;
        }

        UpdateTileUV(tile);
    }

    void PaintASingleElementTile(PATile tile, List<PATile> postProcessMultiElementTiles = null)
    {
        if (tile.element.IsMultiElement())
            return;

        PATile[] nTiles = GetNeighboringTilesNxN(tile, 1);
        int leftBottomValue = 0, leftValue = 0, leftTopValue = 0, topValue = 0,
            rightTopValue = 0, rightValue = 0, rightBottomValue = 0, bottomValue = 0;
        
        PATile leftBottomTile = nTiles[0];
        PATile leftTile = nTiles[1];
        PATile leftTopTile = nTiles[2];
        PATile topTile = nTiles[3];
        PATile rightTopTile = nTiles[4];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];

        int t = 0;
        int elementValue = 0;
        int fireValue = tile.element.FireValue;
        int woodValue = tile.element.WoodValue;
        QtrTileElementType qtrTileElementType = QtrTileElementType.Base;

        TileElementType elementType = TileElementType.None;
        if (fireValue > 0)
        {
            elementType = TileElementType.Fire;
            qtrTileElementType = QtrTileElementType.Fire;
        }
        else if (woodValue > 0)
        {
            elementType = TileElementType.Wood;
            qtrTileElementType = QtrTileElementType.Wood;
        }

        t = elementValue = tile.element.GetElementPaintBrushType(elementType);
        if (leftBottomTile != null)
            leftBottomValue = leftBottomTile.element.GetElementPaintBrushType(elementType);
        if (leftTile != null)
            leftValue = leftTile.element.GetElementPaintBrushType(elementType);
        if (leftTopTile != null)
            leftTopValue = leftTopTile.element.GetElementPaintBrushType(elementType);
        if (topTile != null)
            topValue = topTile.element.GetElementPaintBrushType(elementType);
        if (rightTopTile != null)
            rightTopValue = rightTopTile.element.GetElementPaintBrushType(elementType);
        if (rightTile != null)
            rightValue = rightTile.element.GetElementPaintBrushType(elementType);
        if (rightBottomTile != null)
            rightBottomValue = rightBottomTile.element.GetElementPaintBrushType(elementType);
        if (bottomTile != null)
            bottomValue = bottomTile.element.GetElementPaintBrushType(elementType);

        if (leftValue < elementValue && bottomValue < elementValue)
        {
            CalcTileBits(t, tile, 2);//左下角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(QtrTileElementType.Base, QtrTileElementType.Base, qtrTileElementType, QtrTileElementType.Base);
        }
        else if (leftValue < elementValue && topValue < elementValue)
        {
            CalcTileBits(t, tile, 1);//左上角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(QtrTileElementType.Base, QtrTileElementType.Base, QtrTileElementType.Base, qtrTileElementType);
        }
        else if (rightValue < elementValue && bottomValue < elementValue)
        {
            CalcTileBits(t, tile, 4);//右下角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(QtrTileElementType.Base, qtrTileElementType, QtrTileElementType.Base, QtrTileElementType.Base);
        }
        else if (topValue < elementValue && rightValue < elementValue)
        {
            CalcTileBits(t, tile, 8);//右上角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Base, QtrTileElementType.Base, QtrTileElementType.Base);
        }


        //else if (topValue < elementValue && leftTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(QtrTileElementType.Sand, QtrTileElementType.Base, QtrTileElementType.Base, qtrTileElementType);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}

        else if (leftValue < elementValue)
        {
            CalcTileBits(t, tile, 3);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(QtrTileElementType.Base, QtrTileElementType.Base, qtrTileElementType, qtrTileElementType);
        }
        else if (topValue < elementValue)
        {
            CalcTileBits(t, tile, 9);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Base, QtrTileElementType.Base, qtrTileElementType);
        }
        else if (rightValue < elementValue)
        {
            CalcTileBits(t, tile, 12);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, QtrTileElementType.Base, QtrTileElementType.Base);
        }
        else if (bottomValue < elementValue)
        {
            CalcTileBits(t, tile, 6);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(QtrTileElementType.Base, qtrTileElementType, qtrTileElementType, QtrTileElementType.Base);
        }

        //else if (leftBottomValue < elementValue && bottomTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(QtrTileElementType.Base, qtrTileElementType, qtrTileElementType, QtrTileElementType.Sand);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        //else if (leftBottomValue < elementValue && leftTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(QtrTileElementType.Base, QtrTileElementType.Sand, qtrTileElementType, qtrTileElementType);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        //else if (rightBottomValue < elementValue && rightTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, QtrTileElementType.Sand, QtrTileElementType.Base);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        //else if (rightBottomValue < elementValue && bottomTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(QtrTileElementType.Sand, qtrTileElementType, qtrTileElementType, QtrTileElementType.Base);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        //else if (rightTopValue < elementValue && topTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Sand, QtrTileElementType.Base, qtrTileElementType);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        //else if (rightTopValue < elementValue && rightTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, QtrTileElementType.Base, QtrTileElementType.Sand);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        //else if (leftTopValue < elementValue && leftTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(QtrTileElementType.Sand, QtrTileElementType.Base, qtrTileElementType, qtrTileElementType);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        //else if (leftTopValue < elementValue && topTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.BigCorner;
        //    tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Base, QtrTileElementType.Sand, qtrTileElementType);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}

        else if (leftBottomValue < elementValue)
        {
            CalcTileBits(t, tile, 7);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(QtrTileElementType.Base, qtrTileElementType, qtrTileElementType, qtrTileElementType);
        }
        else if (leftTopValue < elementValue)
        {
            CalcTileBits(t, tile, 11);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Base, qtrTileElementType, qtrTileElementType);
        }
        else if (rightTopValue < elementValue)
        {
            CalcTileBits(t, tile, 13);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, QtrTileElementType.Base, qtrTileElementType);
        }
        else if (rightBottomValue < elementValue)
        {
            CalcTileBits(t, tile, 14);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, qtrTileElementType, QtrTileElementType.Base);
        }

        //else if (rightBottomTile.element.IsMultiAndEqualElement() &&
        //    rightTile.element.IsMultiAndEqualElement() &&
        //    bottomTile.element.IsMultiAndEqualElement())
        //{
        //    tile.SetTileProp(t, SandBrush, 14);
        //    tile.tileSetType = TileSetType.Full;
        //    tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, qtrTileElementType, QtrTileElementType.Sand);
        //    UpdateTileUV(tile);
        //}
        //else if (leftBottomTile.element.IsMultiAndEqualElement() &&
        //    leftTile.element.IsMultiAndEqualElement() &&
        //    bottomTile.element.IsMultiAndEqualElement())
        //{
        //    tile.SetTileProp(t, SandBrush, 7);
        //    tile.tileSetType = TileSetType.Full;
        //    tile.SetQtrTiles(QtrTileElementType.Sand, qtrTileElementType, qtrTileElementType, qtrTileElementType);
        //    UpdateTileUV(tile);
        //}
        //else if (rightTopTile.element.IsMultiAndEqualElement() &&
        //    rightTile.element.IsMultiAndEqualElement() &&
        //    topTile.element.IsMultiAndEqualElement())
        //{
        //    tile.SetTileProp(t, SandBrush, 13);
        //    tile.tileSetType = TileSetType.Full;
        //    tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, QtrTileElementType.Sand, qtrTileElementType);
        //    UpdateTileUV(tile);
        //}
        //else if (leftTopTile.element.IsMultiAndEqualElement() &&
        //    leftTile.element.IsMultiAndEqualElement() &&
        //    topTile.element.IsMultiAndEqualElement())
        //{
        //    tile.SetTileProp(t, SandBrush, 11);
        //    tile.tileSetType = TileSetType.Full;
        //    tile.SetQtrTiles(qtrTileElementType, QtrTileElementType.Sand, qtrTileElementType, qtrTileElementType);
        //    UpdateTileUV(tile);
        //}
        //else if (rightBottomTile.element.IsMultiAndEqualElement() &&
        //    (rightTile.element.IsSingleElement() && rightTile.tileSetType == TileSetType.BigCorner) &&
        //    bottomTile.element.IsMultiAndEqualElement())
        //{
        //    tile.SetTileProp(t, SandBrush, 14);
        //    tile.tileSetType = TileSetType.Full;
        //    tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, qtrTileElementType, QtrTileElementType.Sand);
        //    UpdateTileUV(tile);
        //}
        
        //else if(leftBottomTile.element.IsSingleElement() && 
        //    leftTile.element.IsSingleElement() && 
        //    leftTopTile.element.IsSingleElement() && 
        //    topTile.element.IsSingleElement() && 
        //    rightTopTile.element.IsSingleElement() && 
        //    rightTile.element.IsSingleElement() && 
        //    rightBottomTile.element.IsSingleElement() && 
        //    bottomTile.element.IsSingleElement())
        //{
        //    PaintNormalTile(tile, t);
        //    tile.tileSetType = TileSetType.Full;
        //    tile.SetQtrTiles(qtrTileElementType);
        //}
        //else if (rightBottomTile.element.IsMultiElement() && bottomTile.element.IsMultiElement())
        //{
        //    tile.tileSetType = TileSetType.Full;
        //    tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, qtrTileElementType, QtrTileElementType.None);
        //    postProcessMultiElementTiles.Add(tile);
        //    return;
        //}
        else
        {
            PaintNormalTile(tile, t);
            tile.tileSetType = TileSetType.Full;
            tile.SetQtrTiles(qtrTileElementType);
        }

        //高阶属性地表都设置成一样带属性的
        if(t != FireLevel1Brush && t != WoodLevel1Brush)
            tile.SetQtrTiles(qtrTileElementType);
        
    }
   
    void PaintPostProcessMultiElementTile(PATile tile)
    {
        TilePaintSample tps = GetFitTilePaintSample(tile);
        if (tps != null)
        {
            tile.SetTileProp(tps.type, tps.toType, tps.bits);
        }
        else
        {
            TileMixConfigAsset.TileMixConfig mixConfig = GetTileMixConfig(tile);
            if (mixConfig == null)
            {
                Debug.LogError("mixConfig == null tile.x = " + tile.x + " tile.y = " + tile.y);
                tile.SetTileProp(0, 0, 0);
                UpdateTileUV(tile);
                return;
            }

            tile.SetTileProp(SandBrush, SandBrush, 0, mixConfig.tilesetIndex, (UVRotateType)mixConfig.rotateType);
        }
        UpdateTileUV(tile);
    }
   
    // tile只缺少一个确定的qtrtile
    int IsTileMiss1(PATile tile)
    {
        int index = -1;
        int count = 0;
        for (int i = 0; i < tile.qtrTiles.Length; i++ )
        {
            if (tile.qtrTiles[i] == QtrTileElementType.None)
            {
                index = i;
                count++;
            } 
        }
        if (count == 1)
            return index;
        else
            return -1;
    }

    //如果tile只有一个qtrtile是未知的 那么未知的qtrtile直接设置成融合属性
    void ProcessMiss1Tile(PATile tile)
    {
        int miss1Index = IsTileMiss1(tile);
        if (miss1Index == -1)
            return;

        if(tile.element.IsMultiElement())
            tile.qtrTiles[miss1Index] = QtrTileElementType.Sand;
    }

    bool ProcessMultiElementTile(PATile tile)
    {
        if (tile.IsQtrTilesSet())
            return true;

        PATile[] nTiles = GetNeighboringTilesNxN(tile, 1);

        PATile leftBottomTile = nTiles[0];
        PATile leftTile = nTiles[1];
        PATile leftTopTile = nTiles[2];
        PATile topTile = nTiles[3];
        PATile rightTopTile = nTiles[4];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];

        QtrTileElementType qte0 = bottomTile.qtrTiles[1];
        QtrTileElementType qte1 = leftBottomTile.qtrTiles[2];
        QtrTileElementType qte2 = leftTile.qtrTiles[3];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte2;

        qte0 = leftTile.qtrTiles[2];
        qte1 = leftTopTile.qtrTiles[3];
        qte2 = topTile.qtrTiles[0];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte2;

        qte0 = topTile.qtrTiles[3];
        qte1 = rightTopTile.qtrTiles[0];
        qte2 = rightTile.qtrTiles[1];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte2;

        qte0 = rightTile.qtrTiles[0];
        qte1 = rightBottomTile.qtrTiles[1];
        qte2 = bottomTile.qtrTiles[2];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte2;

        ProcessMiss1Tile(tile);

        if(!tile.IsQtrTilesSet())
            return false;
        return true;
    }

    TileMixConfigAsset.TileMixConfig GetTileMixConfig(PATile tile)
    {
        foreach(var config in ConfigDataBase.instance.TileMixConfigAsset.configs)
        {
            if (config.qtrTile0 == (int)tile.qtrTiles[0] &&
                config.qtrTile1 == (int)tile.qtrTiles[1] &&
                config.qtrTile2 == (int)tile.qtrTiles[2] &&
                config.qtrTile3 == (int)tile.qtrTiles[3])
                return config;
        }

        return null;
    }

    public void PaintATileDecal(PATile tile)
    {
        if (tile == null || tile.decalTilesetIndex != -1)
            return;

        //融合处不能贴花
        if (!tile.IsElementFull())
            return;

        PATile[] nTiles = GetNeighboringTilesNxN(tile, 1);
        PATile leftTile = nTiles[1];
        PATile topTile = nTiles[3];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];
        foreach (var config in ConfigDataBase.instance.DecalConfigAsset.configs)
        {
            if (tile.decalTilesetIndex != -1)
                return;
            if (config.elementType != (int)tile.element.GetMaxElementType())
                continue;
            if (!config.elementValue.Contains(tile.element.GetMaxElementValue()))
                continue;
            TileSetType tileSetType = (TileSetType)config.tileSetType;
            if (tile.tileSetType != tileSetType)
                continue;
            //float rate = (config.maxRate - tile.distance * config.atten);
            int rate = config.rate;
            int randomValue =  RandomManager.instance.Range(0,100);
            if (rate < randomValue)
                continue;
            int randomRotate = RandomManager.instance.Range(0,4);
            UVRotateType rotateType = (UVRotateType)randomRotate;
            if (config.decalType == (int)TileDecalType.Decal_2)
            {
                if(config.tileSetIndex.Count != 2)
                {
                    Debug.LogError("config.tileSetIndex.Count != 2");
                    continue;
                }
                if (rotateType == UVRotateType._90)
                {
                    if (rightTile == null || rightTile.decalTilesetIndex != -1)
                        continue;
                    if (rightTile.tileSetType != tileSetType)
                        continue;

                    tile.decalTilesetIndex = config.tileSetIndex[0];
                    rightTile.decalTilesetIndex = config.tileSetIndex[1];
                    PaintTileDecal(rightTile,rotateType);
                }
                else if (rotateType == UVRotateType.None)
                {
                    if (topTile == null || topTile.decalTilesetIndex != -1)
                        continue;
                    if (topTile.tileSetType != tileSetType)
                        continue;
                    tile.decalTilesetIndex = config.tileSetIndex[0];
                    topTile.decalTilesetIndex = config.tileSetIndex[1];
                    PaintTileDecal(topTile,rotateType);
                }
                else if (rotateType == UVRotateType._180)
                {
                    if (bottomTile == null || bottomTile.decalTilesetIndex != -1)
                        continue;
                    if (bottomTile.tileSetType != tileSetType)
                        continue;
                    tile.decalTilesetIndex = config.tileSetIndex[0];
                    bottomTile.decalTilesetIndex = config.tileSetIndex[1];
                    PaintTileDecal(bottomTile,rotateType);
                }
                else if (rotateType == UVRotateType._270)
                {
                    if (leftTile == null || leftTile.decalTilesetIndex != -1)
                        continue;
                    if (leftTile.tileSetType != tileSetType)
                        continue;
                    tile.decalTilesetIndex = config.tileSetIndex[0];
                    leftTile.decalTilesetIndex = config.tileSetIndex[1];
                    PaintTileDecal(leftTile,rotateType);
                }
                PaintTileDecal(tile,rotateType);
            }
            else if (config.decalType == (int)TileDecalType.Decal_4)
            {
                if (rightTile == null || rightTile .decalTilesetIndex != -1 ||
                    rightBottomTile == null || rightBottomTile.decalTilesetIndex != -1 ||
                    bottomTile == null || bottomTile.decalTilesetIndex != -1)
                    continue;
                if (rightTile.tileSetType != tileSetType)
                    continue;
                if (rightBottomTile.tileSetType != tileSetType)
                    continue;
                if (bottomTile.tileSetType != tileSetType)
                    continue;

                if (config.tileSetIndex.Count != 4)
                {
                    Debug.LogError("config.tileSetIndex.Count != 4");
                    continue;
                }
                    
                if(rotateType == UVRotateType._90)
                {
                    tile.decalTilesetIndex = config.tileSetIndex[0];
                    rightTile.decalTilesetIndex = config.tileSetIndex[1];
                    bottomTile.decalTilesetIndex = config.tileSetIndex[2];
                    rightBottomTile.decalTilesetIndex = config.tileSetIndex[3];
                }
                else if(rotateType == UVRotateType.None)
                {
                    tile.decalTilesetIndex = config.tileSetIndex[1];
                    rightTile.decalTilesetIndex = config.tileSetIndex[3];
                    bottomTile.decalTilesetIndex = config.tileSetIndex[0];
                    rightBottomTile.decalTilesetIndex = config.tileSetIndex[2];
                }
                else if (rotateType == UVRotateType._270)
                {
                    tile.decalTilesetIndex = config.tileSetIndex[3];
                    rightTile.decalTilesetIndex = config.tileSetIndex[2];
                    bottomTile.decalTilesetIndex = config.tileSetIndex[1];
                    rightBottomTile.decalTilesetIndex = config.tileSetIndex[0];
                }
                else if (rotateType == UVRotateType._180)
                {
                    tile.decalTilesetIndex = config.tileSetIndex[2];
                    rightTile.decalTilesetIndex = config.tileSetIndex[0];
                    bottomTile.decalTilesetIndex = config.tileSetIndex[3];
                    rightBottomTile.decalTilesetIndex = config.tileSetIndex[1];
                }
                PaintTileDecal(tile, rotateType);
                PaintTileDecal(rightTile, rotateType);
                PaintTileDecal(bottomTile, rotateType);
                PaintTileDecal(rightBottomTile, rotateType);
            }
            else
            {
                if (config.tileSetIndex.Count != 1)
                {
                    Debug.LogError("config.tileSetIndex.Count != 1");
                    continue;
                }
                tile.decalTilesetIndex = config.tileSetIndex[0];
                PaintTileDecal(tile,rotateType);
            }
        }
    }

    public void PaintTiles(ref List<PATile> tiles)
    {
        //先处理单属性地格 多属性的后处理
        //tiles.Sort((PATile t1,PATile t2) =>
        //{
        //    if(t1.element.IsMultiElement() && !t2.element.IsMultiElement())
        //        return 1;
        //    else if (!t1.element.IsMultiElement() && t2.element.IsMultiElement())
        //        return -1;
        //    return 0;
        //});

        //先处理单属性地格 多属性的后处理
        List<PATile> multiElementTiles = new List<PATile>();
        List<PATile> postProcessMultiElementTiles = new List<PATile>();
        foreach (var tile in tiles)
        {
            if (tile.element.IsMultiElement())
            {
                multiElementTiles.Add(tile);
                continue;
            }
            PaintASingleElementTile(tile, postProcessMultiElementTiles);
        }

        foreach (var tile in multiElementTiles)
            PaintAMultiElementTile(tile,postProcessMultiElementTiles);

        List<PATile> collectUnsetTiles0 = new List<PATile>();
        //List<PATile> collectUnsetTiles1 = new List<PATile>();
        
        foreach (var tile in postProcessMultiElementTiles)
            if (!ProcessMultiElementTile(tile))
                collectUnsetTiles0.Add(tile);

        if (collectUnsetTiles0.Count > 0)
            Debug.LogError("collectUnsetTiles0.Count  " + collectUnsetTiles0.Count);

        //int processTimes = 0;
        //while (true)
        //{
        //    collectUnsetTiles1.Clear();
        //    foreach (var tile in collectUnsetTiles0)
        //        if (!ProcessMultiElementTile(tile))
        //            collectUnsetTiles1.Add(tile);

        //    collectUnsetTiles0.Clear();
        //    foreach (var tile in collectUnsetTiles1)
        //        if (!ProcessMultiElementTile(tile))
        //            collectUnsetTiles0.Add(tile);

        //    if (collectUnsetTiles0.Count == 0 && collectUnsetTiles1.Count == 0)
        //        break;
        //    if (processTimes++ > 5)
        //    {
        //        Debug.LogError("processTimes++ > 5");
        //        break;
        //    } 
        //}

        foreach (var tile in postProcessMultiElementTiles)
            PaintPostProcessMultiElementTile(tile);
    }
}