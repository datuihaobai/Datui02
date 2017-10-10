using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public partial class PATileTerrain
{
    public const int BaseBrush = 0;
    public const int FireLevel1Brush = 1;
    public const int FireLevel2Brush = 2;
    public const int FireLevel3Brush = 3;
    public const int WoodLevel1Brush = 4;
    public const int WoodLevel2Brush = 5;
    public const int WoodLevel3Brush = 6;
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
        if (tile == null)
            return TileElementState.None;
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
            if (tile == null)
                continue;
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

    // index=0=lb index=2=lt index=4=rt index=6=rb
    bool IsCornerMix(PATile[] nTiles,int index)
    {
        int i = 0;
        bool result = true;
        foreach(var nTile in nTiles)
        {
            if (i++ == index)
                result &= PATile.IsSingleElement(nTile);
            else
                result &= PATile.IsMultiElement(nTile);

            if (!result)
                return false;
        }
        return true;
    }

    bool IsFullMix(PATile[] nTiles)
    {
        bool result = true;
        foreach(var nTile in nTiles)
        {
            result &= PATile.IsMultiElement(nTile);
            if (!result)
                return false;
        }
        return true;
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

        TileElementState leftTopElementState = GetTileElementState(leftTopTile);
        TileElementState leftBottomElementState = GetTileElementState(leftBottomTile);
        TileElementState rightTopElementState = GetTileElementState(rightTopTile);
        TileElementState rightBottomElementState = GetTileElementState(rightBottomTile);

        if (PATile.IsSingleElement(leftTopTile) &&
            PATile.IsSingleElement(leftTile) &&
            PATile.IsSingleElement(topTile) &&
            PATile.IsMultiElement(rightBottomTile) &&
            PATile.IsTileSetType(leftTopTile,TileSetType.Full) &&
            PATile.IsTileSetType(leftTile,TileSetType.Full) &&
            PATile.IsTileSetType(topTile,TileSetType.Full))
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
        else if (PATile.IsSingleElement(rightTopTile) &&
            PATile.IsSingleElement(rightTile) &&
            PATile.IsSingleElement(topTile) &&
            PATile.IsMultiElement(leftBottomTile) &&
            PATile.IsTileSetType(rightTopTile,TileSetType.Full) &&
            PATile.IsTileSetType(rightTile,TileSetType.Full) &&
            PATile.IsTileSetType(topTile,TileSetType.Full) )
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
        else if (PATile.IsSingleElement(leftBottomTile) &&
            PATile.IsSingleElement(leftTile) &&
            PATile.IsSingleElement(bottomTile) &&
            PATile.IsMultiElement(rightTopTile) &&
            PATile.IsTileSetType(leftBottomTile,TileSetType.Full)&&
            PATile.IsTileSetType(leftTile,TileSetType.Full) &&
            PATile.IsTileSetType(bottomTile,TileSetType.Full))
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
        else if (PATile.IsSingleElement(rightBottomTile) &&
            PATile.IsSingleElement(rightTile) &&
            PATile.IsSingleElement(bottomTile) &&
            PATile.IsMultiElement(leftTopTile) &&
            PATile.IsTileSetType(rightBottomTile,TileSetType.Full) &&
            PATile.IsTileSetType(rightTile,TileSetType.Full) &&
            PATile.IsTileSetType(bottomTile,TileSetType.Full))
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
        else if (IsCornerMix(nTiles, 2))
        {
            tile.SetTileProp(t, SandBrush, 4);
            tile.SetQtrTiles(QtrTileElementType.Sand, qtrTileElementType, QtrTileElementType.Sand, QtrTileElementType.Sand);
            tile.tileSetType = TileSetType.Corner;
        }
        else if (IsCornerMix(nTiles, 4))
        {
            tile.SetTileProp(t, SandBrush, 2);
            tile.SetQtrTiles(QtrTileElementType.Sand, QtrTileElementType.Sand, qtrTileElementType, QtrTileElementType.Sand);
            tile.tileSetType = TileSetType.Corner;
        }
        else if (IsCornerMix(nTiles, 6))
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

    void PaintASingleElementTile(PATile tile, List<PATile> postProcessSingleElementTiles = null)
    {
        if (tile.element.IsMultiElement())
            return;

        PATile[] nTiles = GetNeighboringTilesNxN(tile, 1);
        int leftBottomValue = 99, leftValue = 99, leftTopValue = 99, topValue = 99,
            rightTopValue = 99, rightValue = 99, rightBottomValue = 99, bottomValue = 99;
        
        PATile leftBottomTile = nTiles[0];
        PATile leftTile = nTiles[1];
        PATile leftTopTile = nTiles[2];
        PATile topTile = nTiles[3];
        PATile rightTopTile = nTiles[4];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];

        bool needPostProcess = false;
        foreach (var nTile in nTiles)
        {
            if (PATileElement.IsMultiElemenTile(nTile))
                needPostProcess = true;
        }
        if (needPostProcess)
            postProcessSingleElementTiles.Add(tile);

        int t = 0;
        int elementValue = 0;
        int fireValue = tile.element.FireValue;
        int woodValue = tile.element.WoodValue;
        QtrTileElementType qtrTileElementType = QtrTileElementType.Base;
        QtrTileElementType mixQtrTileElementType = QtrTileElementType.Base;

        TileElementType elementType = TileElementType.None;
        if (fireValue > 0)
            elementType = TileElementType.Fire;
        else if (woodValue > 0)
            elementType = TileElementType.Wood;

        t = elementValue = tile.element.GetSingleElementPaintBrushType(elementType);
        if (leftBottomTile != null)
            leftBottomValue = leftBottomTile.GetSingleElementPaintBrushType(elementType);
        if (leftTile != null)
            leftValue = leftTile.GetSingleElementPaintBrushType(elementType);
        if (leftTopTile != null)
            leftTopValue = leftTopTile.GetSingleElementPaintBrushType(elementType);
        if (topTile != null)
            topValue = topTile.GetSingleElementPaintBrushType(elementType);
        if (rightTopTile != null)
            rightTopValue = rightTopTile.GetSingleElementPaintBrushType(elementType);
        if (rightTile != null)
            rightValue = rightTile.GetSingleElementPaintBrushType(elementType);
        if (rightBottomTile != null)
            rightBottomValue = rightBottomTile.GetSingleElementPaintBrushType(elementType);
        if (bottomTile != null)
            bottomValue = bottomTile.GetSingleElementPaintBrushType(elementType);

        if (t == FireLevel1Brush)
        {
            qtrTileElementType = QtrTileElementType.Fire;
            mixQtrTileElementType = QtrTileElementType.Base;
        } 
        else if(t == FireLevel2Brush)
        {
            qtrTileElementType = QtrTileElementType.Fire2;
            mixQtrTileElementType = QtrTileElementType.Fire;
        }
        else if (t == FireLevel3Brush)
        {
            qtrTileElementType = QtrTileElementType.Fire3;
            mixQtrTileElementType = QtrTileElementType.Fire2;
        }
        else if (t == WoodLevel1Brush)
        {
            qtrTileElementType = QtrTileElementType.Wood;
            mixQtrTileElementType = QtrTileElementType.Base;
        }
        else if (t == WoodLevel2Brush)
        {
            qtrTileElementType = QtrTileElementType.Wood2;
            mixQtrTileElementType = QtrTileElementType.Wood;
        }
        else if (t == WoodLevel3Brush)
        {
            qtrTileElementType = QtrTileElementType.Wood3 ;
            mixQtrTileElementType = QtrTileElementType.Wood2;
        }

        if (leftValue < elementValue && bottomValue < elementValue)
        {
            CalcTileBits(t, tile, 2);//左下角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(mixQtrTileElementType, mixQtrTileElementType, qtrTileElementType, mixQtrTileElementType);
        }
        else if (leftValue < elementValue && topValue < elementValue)
        {
            CalcTileBits(t, tile, 1);//左上角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(mixQtrTileElementType, mixQtrTileElementType, mixQtrTileElementType, qtrTileElementType);
        }
        else if (rightValue < elementValue && bottomValue < elementValue)
        {
            CalcTileBits(t, tile, 4);//右下角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(mixQtrTileElementType, qtrTileElementType, mixQtrTileElementType, mixQtrTileElementType);
        }
        else if (topValue < elementValue && rightValue < elementValue)
        {
            CalcTileBits(t, tile, 8);//右上角
            tile.tileSetType = TileSetType.Corner;
            tile.SetQtrTiles(qtrTileElementType, mixQtrTileElementType, mixQtrTileElementType, mixQtrTileElementType);
        }

        else if (leftValue < elementValue)
        {
            CalcTileBits(t, tile, 3);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(mixQtrTileElementType, mixQtrTileElementType, qtrTileElementType, qtrTileElementType);
        }
        else if (topValue < elementValue)
        {
            CalcTileBits(t, tile, 9);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(qtrTileElementType, mixQtrTileElementType, mixQtrTileElementType, qtrTileElementType);
        }
        else if (rightValue < elementValue)
        {
            CalcTileBits(t, tile, 12);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, mixQtrTileElementType, mixQtrTileElementType);
        }
        else if (bottomValue < elementValue)
        {
            CalcTileBits(t, tile, 6);
            tile.tileSetType = TileSetType.Edge;
            tile.SetQtrTiles(mixQtrTileElementType, qtrTileElementType, qtrTileElementType, mixQtrTileElementType);
        }

        else if (leftBottomValue < elementValue)
        {
            CalcTileBits(t, tile, 7);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(mixQtrTileElementType, qtrTileElementType, qtrTileElementType, qtrTileElementType);
        }
        else if (leftTopValue < elementValue)
        {
            CalcTileBits(t, tile, 11);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(qtrTileElementType, mixQtrTileElementType, qtrTileElementType, qtrTileElementType);
        }
        else if (rightTopValue < elementValue)
        {
            CalcTileBits(t, tile, 13);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, mixQtrTileElementType, qtrTileElementType);
        }
        else if (rightBottomValue < elementValue)
        {
            CalcTileBits(t, tile, 14);
            tile.tileSetType = TileSetType.BigCorner;
            tile.SetQtrTiles(qtrTileElementType, qtrTileElementType, qtrTileElementType, mixQtrTileElementType);
        }
        else
        {
            PaintNormalTile(tile, t);
            tile.tileSetType = TileSetType.Full;
            tile.SetQtrTiles(qtrTileElementType);
        }
    }
   
    void PaintPostProcessMultiElementTile(PATile tile)
    {
        if (tile.QtrTileTypeCount() >= 3)
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
        else
        {
            tile.SetTilePropByQtrTile();
        }
        UpdateTileUV(tile);
    }
   
    void PaintPostProcessSingleElementTile(PATile tile)
    {
        tile.SetTilePropByQtrTile();
        UpdateTileUV(tile);
    }

    // tile只有一个未确定的qtrtile
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

    // 融合时只考虑一级属性的情况 二级及以上都当作一级处理
    static QtrTileElementType GetMixQtrTileElementType(QtrTileElementType qtet)
    {
        if (qtet == QtrTileElementType.Fire2 || qtet == QtrTileElementType.Fire3)
            return QtrTileElementType.Fire;
        else if (qtet == QtrTileElementType.Wood2 || qtet == QtrTileElementType.Wood3)
            return QtrTileElementType.Wood;
        return qtet;
    }

    static QtrTileElementType GetMixQtrTileElementType(PATile tile,int qtrIndex)
    {
        if (tile == null)
            return QtrTileElementType.None;
        else 
            return GetMixQtrTileElementType(tile.qtrTiles[qtrIndex]);
    }

    void ProcessMultiElementTile(PATile tile)
    {
        if (tile.IsQtrTilesSet())
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

        QtrTileElementType qte0 = GetMixQtrTileElementType(bottomTile, 1);
        QtrTileElementType qte1 = GetMixQtrTileElementType(leftBottomTile, 2);
        QtrTileElementType qte2 = GetMixQtrTileElementType(leftTile, 3);
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte2;

        qte0 = GetMixQtrTileElementType(leftTile, 2);
        qte1 = GetMixQtrTileElementType(leftTopTile, 3);
        qte2 = GetMixQtrTileElementType(topTile, 0);
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte2;

        qte0 = GetMixQtrTileElementType(topTile, 3);
        qte1 = GetMixQtrTileElementType(rightTopTile, 0);
        qte2 = GetMixQtrTileElementType(rightTile, 1);
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte2;

        qte0 = GetMixQtrTileElementType(rightTile, 0);
        qte1 = GetMixQtrTileElementType(rightBottomTile, 1);
        qte2 = GetMixQtrTileElementType(bottomTile, 2);
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte2;
    }

    void ProcessSingleElememtTile(PATile tile)
    {
        PATile[] nTiles = GetNeighboringTilesNxN(tile, 1);

        PATile leftBottomTile = nTiles[0];
        PATile leftTile = nTiles[1];
        PATile leftTopTile = nTiles[2];
        PATile topTile = nTiles[3];
        PATile rightTopTile = nTiles[4];
        PATile rightTile = nTiles[5];
        PATile rightBottomTile = nTiles[6];
        PATile bottomTile = nTiles[7];

        QtrTileElementType qte0 = PATile.GetQtrTileElementType(bottomTile, 1);
        QtrTileElementType qte1 = PATile.GetQtrTileElementType(leftBottomTile, 2);
        QtrTileElementType qte2 = PATile.GetQtrTileElementType(leftTile, 3);
        QtrTileElementType preQte = tile.qtrTiles[0];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[0] = qte2;

        qte0 = PATile.GetQtrTileElementType(leftTile, 2);
        qte1 = PATile.GetQtrTileElementType(leftTopTile, 3);
        qte2 = PATile.GetQtrTileElementType(topTile, 0);
        preQte = tile.qtrTiles[1];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[1] = qte2;

        qte0 = PATile.GetQtrTileElementType(topTile, 3);
        qte1 = PATile.GetQtrTileElementType(rightTopTile, 0);
        qte2 = PATile.GetQtrTileElementType(rightTile, 1);
        preQte = tile.qtrTiles[2];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[2] = qte2;

        qte0 = PATile.GetQtrTileElementType(rightTile, 0);
        qte1 = PATile.GetQtrTileElementType(rightBottomTile, 1);
        qte2 = PATile.GetQtrTileElementType(bottomTile, 2);
        preQte = tile.qtrTiles[3];
        if (qte0 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte0;
        else if (qte1 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte1;
        else if (qte2 != QtrTileElementType.None)
            tile.qtrTiles[3] = qte2;

        if (!tile.IsQtrTilesSet())
            Debug.LogWarning("ProcessSingleElememtTile !tile.IsQtrTilesSet() tile.x = " + tile.x + " tile.y " + tile.y);
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
            if (config.elementType != (int)tile.element.GetDecalSuitTileType())
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
                    if (rightTile == null || rightTile.decalTilesetIndex != -1 || !rightTile.IsElementFull())
                        continue;
                    if (rightTile.tileSetType != tileSetType)
                        continue;

                    tile.decalTilesetIndex = config.tileSetIndex[0];
                    rightTile.decalTilesetIndex = config.tileSetIndex[1];
                    PaintTileDecal(rightTile,rotateType);
                }
                else if (rotateType == UVRotateType.None)
                {
                    if (topTile == null || topTile.decalTilesetIndex != -1 || !topTile.IsElementFull())
                        continue;
                    if (topTile.tileSetType != tileSetType)
                        continue;
                    tile.decalTilesetIndex = config.tileSetIndex[0];
                    topTile.decalTilesetIndex = config.tileSetIndex[1];
                    PaintTileDecal(topTile,rotateType);
                }
                else if (rotateType == UVRotateType._180)
                {
                    if (bottomTile == null || bottomTile.decalTilesetIndex != -1 || !bottomTile.IsElementFull())
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
                if (rightTile == null || rightTile .decalTilesetIndex != -1 || !rightTile.IsElementFull() ||
                    rightBottomTile == null || rightBottomTile.decalTilesetIndex != -1 || !rightBottomTile.IsElementFull() ||
                    bottomTile == null || bottomTile.decalTilesetIndex != -1 || !bottomTile.IsElementFull())
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
        List<PATile> multiElementTiles = new List<PATile>();
        List<PATile> postProcessMultiElementTiles = new List<PATile>();
        List<PATile> postProcessSingleElementTiles = new List<PATile>();
        foreach (var tile in tiles)
        {
            if (tile.element.IsMultiElement())
            {
                multiElementTiles.Add(tile);
                continue;
            }
            PaintASingleElementTile(tile,postProcessSingleElementTiles);
        }

        foreach (var tile in multiElementTiles)
            PaintAMultiElementTile(tile,postProcessMultiElementTiles);

        foreach (var tile in postProcessMultiElementTiles)
        {
            ProcessMultiElementTile(tile);
            ProcessMiss1Tile(tile);
        } 

        foreach (var tile in postProcessMultiElementTiles)
            PaintPostProcessMultiElementTile(tile);

        foreach (var tile in postProcessSingleElementTiles)
            tile.ProcessMixPerfect(this);

        foreach (var tile in postProcessSingleElementTiles)
            ProcessSingleElememtTile(tile);

        foreach (var tile in postProcessSingleElementTiles)
            PaintPostProcessSingleElementTile(tile);
    }
}