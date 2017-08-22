﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public partial class PATileTerrain
{
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

    public void ShowCrystal(bool isShow)
    {
        if (settings.chunks == null)
            return;

        foreach (PATileTerrainChunk c in settings.chunks)
            c.settings.crystalGo.SetActive(isShow);
    }

    void SetTilesElementSym(PATile[] tiles, int startIndex,int count, 
        TileElementType elementType,int value,bool isReset)
    {
        SetTilesElement(tiles,startIndex,count,elementType,value,isReset);
        SetTilesElementFromTail(tiles, startIndex, count, elementType, value, isReset);
    }

    void SetTilesElementFromTail(PATile[] tiles, int startIndex, int count,
        TileElementType elementType, int value, bool isReset)
    {
        for (int i = tiles.Length - startIndex - 1; i > tiles.Length - startIndex - count - 1; i--)
            SetTileElement(tiles[i], elementType, value, isReset);
    }

    void SetTilesElement(PATile[] tiles, int startIndex,int count,
        TileElementType elementType,int value,bool isReset)
    {
        for (int i = startIndex; i < startIndex + count; i ++)
            SetTileElement(tiles[i], elementType, value, isReset);
    }

    void SetTileElement(PATile tile, TileElementType elementType, int value, bool isReset)
    {
        if (tile == null)
            return;
        if (isReset)
            tile.element.Reset();
        else
        {
            if (elementType == TileElementType.Fire)
                tile.element.AddFire(value);
            else if (elementType == TileElementType.Wood)
                tile.element.AddWood(value);
        }
    }

    void PaintNormalTile(PATile tile, int t, int specifiedIndex = -1, UVRotateType rotateType = UVRotateType.None)
    {
        if (tile == null)
            return;
        tile.type = t;
        tile.toType = t;
        tile.bits = 0;
        UpdateTileUV(tile,specifiedIndex,rotateType);
    }

    void PaintNormalTiles(PATile[] tiles,int t,int startIndex,int count)
    {
        for (int i = startIndex; i < startIndex + count; i++ )
            PaintNormalTile(tiles[i],t);
    }

    void PaintNormalTilesFromTail(PATile[] tiles,int t,int startIndex,int count)
    {
        for (int i = tiles.Length - startIndex - 1; i > tiles.Length - startIndex - count - 1; i--)
            PaintNormalTile(tiles[i], t);
    }

    void PaintNormalTilesSym(PATile[] tiles,int t,int startIndex,int count)
    {
        if (count == 0)
            return;
        PaintNormalTiles(tiles,t,startIndex,count);
        PaintNormalTilesFromTail(tiles, t, startIndex, count);
    }

    PATile GetATile(PATile tile,int offsetX,int offsetY)
    {
        return GetATile(tile.x,tile.y,offsetX,offsetY);
    }

    PATile GetATile(int x,int y,int offsetX,int offsetY)
    {
        PATile[] tiles = GetLineTiles(x,y,offsetX,offsetY,1);
        if (tiles.Length == 1)
            return tiles[0];
        return null;
    }

    PATile[] GetLineTiles(int x,int y,int offsetX,int offsetY,int count)
    {
        PATile[] tiles = new PATile[count];
        for (int i = 0; i < count; i++)
            tiles[i] = null;

        if (y + offsetY < 0 || y + offsetY > settings.yCount - 1)
            return tiles;
        
        int index = 0;
        for (int ix = x + offsetX; ix < x + offsetX + count; ix++)
        {
            if (ix < 0 || ix > settings.xCount - 1) { index++; continue; }
            tiles[index++] = GetTile(ix,y + offsetY);
        }
        return tiles;
    }

    byte GetSymBitsLR(byte bits)
    {
        if (bits == 2)
            return 4;
        else if (bits == 6)
            return 6;
        else if (bits == 7)
            return 14;
        else if (bits == 3)
            return 12;
        else if (bits == 1)
            return 8;
        else if (bits == 11)
            return 13;
        else if (bits == 9)
            return 9;
        return 0;
    }

    byte GetSymBitsTB(byte bits)
    {
        if (bits == 2)
            return 1;
        else if (bits == 6)
            return 9;
        else if (bits == 7)
            return 11;
        else if (bits == 3)
            return 3;
        return 0;
    }

    byte[] GetSymBitsLR(byte[] bits)
    {
        byte[] symBits = new byte[bits.Length];
        for (int i = 0; i < bits.Length; i++ )
            symBits[i] = GetSymBitsLR(bits[i]);
        return symBits;
    }

    byte[] GetSymBitsTB(byte[] bits)
    {
        byte[] symBits = new byte[bits.Length];
        for (int i = 0; i < bits.Length; i++)
            symBits[i] = GetSymBitsTB(bits[i]);
        return symBits;
    }

    void CalcTilesBits(int t,PATile[] tiles,int startIndex,byte[] bits)
    {
        for (int i = startIndex,j = 0; i < startIndex + bits.Length; i++,j++)
            CalcTileBits(t, tiles[i], bits[j]);
    }

    void CalcTilesBitsFromTail(int t, PATile[] tiles, int startIndex, byte[] bits)
    {
        byte[] symBits = GetSymBitsLR(bits);
        for (int i = tiles.Length - startIndex - 1,j = 0;
            i > tiles.Length - startIndex - bits.Length - 1;
            i--,j++)
            CalcTileBits(t, tiles[i], symBits[j]);
    }

    void CalcTilesBitsSymLR(int t, PATile[] tiles, int startIndex, byte[] bits)
    {
        CalcTilesBits(t,tiles,startIndex,bits);
        CalcTilesBitsFromTail(t, tiles, startIndex, bits);
    }

    void CalcTilesBitsSymTB(int t, PATile[] tiles, int startIndex, byte[] bits)
    {
        byte[] symBits = GetSymBitsTB(bits);
        CalcTilesBits(t, tiles, startIndex, symBits);
        CalcTilesBitsFromTail(t, tiles, startIndex, symBits);
    }

    void PaintALine(PATile tile, int xOffset, int yOffset, int count, int t, byte[] bits)
    {
        int normalCount = (count / 2) - bits.Length;
        PATile[] tiles = GetLineTiles(tile.x, tile.y, xOffset, yOffset, count);
        CalcTilesBitsSymLR(t, tiles, 0, bits);
        if (normalCount > 0)
            PaintNormalTilesSym(tiles, t, bits.Length, normalCount);

        tiles = GetLineTiles(tile.x, tile.y, xOffset, Mathf.Abs(yOffset) + 1, count);
        CalcTilesBitsSymTB(t, tiles, 0, bits);
        if (normalCount > 0)
            PaintNormalTilesSym(tiles, t, bits.Length, normalCount);
    }

    public void PaintCrystalLevel1(PATile tile, int t)
    {
        PATile[] line1Tiles = GetLineTiles(tile.x,tile.y,-1,-4,4);
        CalcTileBits(t, line1Tiles[0], 2);
        CalcTileBits(t, line1Tiles[1], 6);
        CalcTileBits(t, line1Tiles[2], 6);
        CalcTileBits(t, line1Tiles[3], 4);

        PATile[] line2Tiles = GetLineTiles(tile.x,tile.y,-2,-3,6);
        CalcTileBits(t, line2Tiles[0], 2);
        CalcTileBits(t, line2Tiles[1], 7);
        PaintNormalTiles(line2Tiles,t,2,2);
        CalcTileBits(t, line2Tiles[4], 14);
        CalcTileBits(t, line2Tiles[5], 4);

        PATile[] line3Tiles = GetLineTiles(tile.x, tile.y, -3, -2, 8);
        CalcTileBits(t, line3Tiles[0], 2);
        CalcTileBits(t, line3Tiles[1], 7);
        PaintNormalTiles(line3Tiles,t,2,4);
        CalcTileBits(t, line3Tiles[6], 14);
        CalcTileBits(t, line3Tiles[7], 4);

        PATile[] line4Tiles = GetLineTiles(tile.x, tile.y, -4, -1, 10);
        CalcTileBits(t, line4Tiles[0], 2);
        CalcTileBits(t, line4Tiles[1], 7);
        PaintNormalTiles(line4Tiles,t,2,6);
        CalcTileBits(t, line4Tiles[8], 14);
        CalcTileBits(t, line4Tiles[9], 4);

        PATile[] line5Tiles = GetLineTiles(tile.x, tile.y, -4, 0, 10);
        CalcTileBits(t, line5Tiles[0], 3);
        PaintNormalTiles(line5Tiles,t,1,8);
        CalcTileBits(t, line5Tiles[9], 12);

        PATile[] line6Tiles = GetLineTiles(tile.x, tile.y, -4, 1, 10);
        CalcTileBits(t, line6Tiles[0], 3);
        PaintNormalTiles(line6Tiles,t,1,8);
        CalcTileBits(t, line6Tiles[9], 12);

        PATile[] line7Tiles = GetLineTiles(tile.x, tile.y, -4, 2, 10);
        CalcTileBits(t, line7Tiles[0], 1);
        CalcTileBits(t, line7Tiles[1], 11);
        PaintNormalTiles(line7Tiles,t,2,6);
        CalcTileBits(t, line7Tiles[8], 13);
        CalcTileBits(t, line7Tiles[9], 8);

        PATile[] line8Tiles = GetLineTiles(tile.x, tile.y, -3, 3, 8);
        CalcTileBits(t, line8Tiles[0], 1);
        CalcTileBits(t, line8Tiles[1], 11);
        PaintNormalTiles(line8Tiles,t,2,4);
        CalcTileBits(t, line8Tiles[6], 13);
        CalcTileBits(t, line8Tiles[7], 8);

        PATile[] line9Tiles = GetLineTiles(tile.x, tile.y, -2, 4, 6);
        CalcTileBits(t, line9Tiles[0], 1);
        CalcTileBits(t, line9Tiles[1], 11);
        PaintNormalTiles(line9Tiles,t,2,2);
        CalcTileBits(t, line9Tiles[4], 13);
        CalcTileBits(t, line9Tiles[5], 8);

        PATile[] line10Tiles = GetLineTiles(tile.x, tile.y, -1, 5, 4);
        CalcTileBits(t, line10Tiles[0], 1);
        CalcTileBits(t, line10Tiles[1], 9);
        CalcTileBits(t, line10Tiles[2], 9);
        CalcTileBits(t, line10Tiles[3], 8);
    }

    public void PaintCrystalLevel2(PATile tile, int t)
    {
        PATile[] line1Tiles = GetLineTiles(tile.x, tile.y, -2, -8, 6);
        CalcTileBits(t, line1Tiles[0], 2);
        CalcTileBits(t, line1Tiles[1], 6);
        CalcTileBits(t, line1Tiles[2], 6);
        CalcTileBits(t, line1Tiles[3], 6);
        CalcTileBits(t, line1Tiles[4], 6);
        CalcTileBits(t, line1Tiles[5], 4);

        PATile[] line2Tiles = GetLineTiles(tile.x, tile.y, -4, -7, 10);
        CalcTileBits(t, line2Tiles[0], 2);
        CalcTileBits(t, line2Tiles[1], 6);
        CalcTileBits(t, line2Tiles[2], 7);
        PaintNormalTiles(line2Tiles, t, 3, 4);
        CalcTileBits(t, line2Tiles[7], 14);
        CalcTileBits(t, line2Tiles[8], 6);
        CalcTileBits(t, line2Tiles[9], 4);

        PATile[] line3Tiles = GetLineTiles(tile.x, tile.y, -5, -6, 12);
        CalcTileBits(t, line3Tiles[0], 2);
        CalcTileBits(t, line3Tiles[1], 7);
        PaintNormalTiles(line3Tiles,t,2,8);
        CalcTileBits(t, line3Tiles[10], 14);
        CalcTileBits(t, line3Tiles[11], 4);

        PATile[] line4Tiles = GetLineTiles(tile.x, tile.y, -6, -5, 14);
        CalcTileBits(t, line4Tiles[0], 2);
        PaintNormalTiles(line4Tiles,t,1,12);
        CalcTileBits(t, line4Tiles[13], 4);

        PATile[] line5Tiles = GetLineTiles(tile.x, tile.y, -7, -4, 16);
        CalcTileBits(t, line5Tiles[0], 2);
        CalcTileBits(t, line5Tiles[1], 7);
        PaintNormalTiles(line5Tiles,t,2,12);
        CalcTileBits(t, line5Tiles[14], 14);
        CalcTileBits(t, line5Tiles[15], 4);

        PATile[] line6Tiles = GetLineTiles(tile.x, tile.y, -7, -3, 16);
        CalcTileBits(t, line6Tiles[0], 3);
        PaintNormalTiles(line6Tiles,t,1,14);
        CalcTileBits(t, line6Tiles[15], 12);

        PATile[] line7Tiles = GetLineTiles(tile.x, tile.y, -8, -2, 18);
        CalcTileBits(t, line7Tiles[0], 2);
        CalcTileBits(t, line7Tiles[1], 7);
        PaintNormalTiles(line7Tiles,t,2,14);
        CalcTileBits(t, line7Tiles[16], 14);
        CalcTileBits(t, line7Tiles[17], 4);

        PATile[] line8Tiles = GetLineTiles(tile.x, tile.y, -8, -1, 18);
        CalcTileBits(t, line8Tiles[0], 3);
        PaintNormalTiles(line8Tiles,t,1,16);
        CalcTileBits(t, line8Tiles[17], 12);

        PATile[] line9Tiles = GetLineTiles(tile.x, tile.y, -8, 0, 18);
        CalcTileBits(t, line9Tiles[0], 3);
        PaintNormalTiles(line9Tiles,t,1,16);
        CalcTileBits(t, line9Tiles[17], 12);

        PATile[] line10Tiles = GetLineTiles(tile.x, tile.y, -8, 1, 18);
        CalcTileBits(t, line10Tiles[0], 3);
        PaintNormalTiles(line10Tiles,t,1,16);
        CalcTileBits(t, line10Tiles[17], 12);

        PATile[] line11Tiles = GetLineTiles(tile.x, tile.y, -8, 2, 18);
        CalcTileBits(t, line11Tiles[0], 3);
        PaintNormalTiles(line11Tiles,t,1,16);
        CalcTileBits(t, line11Tiles[17], 12);

        PATile[] line12Tiles = GetLineTiles(tile.x, tile.y, -8, 3, 18);
        CalcTileBits(t, line12Tiles[0], 1);
        CalcTileBits(t, line12Tiles[1], 11);
        PaintNormalTiles(line12Tiles, t, 2, 14);
        CalcTileBits(t, line12Tiles[16], 13);
        CalcTileBits(t, line12Tiles[17], 8);

        PATile[] line13Tiles = GetLineTiles(tile.x, tile.y, -7, 4, 16);
        CalcTileBits(t, line13Tiles[0], 3);
        PaintNormalTiles(line13Tiles, t, 1, 14);
        CalcTileBits(t, line13Tiles[15], 12);

        PATile[] line14Tiles = GetLineTiles(tile.x, tile.y, -7, 5, 16);
        CalcTileBits(t, line14Tiles[0], 1);
        CalcTileBits(t, line14Tiles[1], 11);
        PaintNormalTiles(line14Tiles, t, 2, 12);
        CalcTileBits(t, line14Tiles[14], 13);
        CalcTileBits(t, line14Tiles[15], 8);

        PATile[] line15Tiles = GetLineTiles(tile.x, tile.y, -6, 6, 14);
        CalcTileBits(t, line15Tiles[0], 1);
        PaintNormalTiles(line15Tiles, t, 1, 12);
        CalcTileBits(t, line15Tiles[13], 8);

        PATile[] line16Tiles = GetLineTiles(tile.x, tile.y, -5, 7, 12);
        CalcTileBits(t, line16Tiles[0], 1);
        CalcTileBits(t, line16Tiles[1], 11);
        PaintNormalTiles(line16Tiles, t, 2, 8);
        CalcTileBits(t, line16Tiles[10], 13);
        CalcTileBits(t, line16Tiles[11], 8);

        PATile[] line17Tiles = GetLineTiles(tile.x, tile.y, -4, 8, 10);
        CalcTileBits(t, line17Tiles[0], 1);
        CalcTileBits(t, line17Tiles[1], 9);
        CalcTileBits(t, line17Tiles[2], 11);
        PaintNormalTiles(line17Tiles, t, 3, 4);
        CalcTileBits(t, line17Tiles[7], 13);
        CalcTileBits(t, line17Tiles[8], 9);
        CalcTileBits(t, line17Tiles[9], 8);

        PATile[] line18Tiles = GetLineTiles(tile.x, tile.y, -2, 9, 6);
        CalcTileBits(t, line18Tiles[0], 1);
        CalcTileBits(t, line18Tiles[1], 9);
        CalcTileBits(t, line18Tiles[2], 9);
        CalcTileBits(t, line18Tiles[3], 9);
        CalcTileBits(t, line18Tiles[4], 9);
        CalcTileBits(t, line18Tiles[5], 8);
    }

    public void PaintCrystalLevel2_B(PATile tile, int t)
    {
        PaintCrystalLevel1(tile, t);
        PATile specifiedTile = GetATile(tile, 1, -3);
        PaintNormalTile(specifiedTile, t, 51, UVRotateType._90);

        specifiedTile = GetATile(tile, 0, -2);
        PaintNormalTile(specifiedTile, t, 52, UVRotateType._90);

        specifiedTile = GetATile(tile, -2, -1);
        PaintNormalTile(specifiedTile, t, 49, UVRotateType._90);
        specifiedTile = GetATile(tile, -1, -1);
        PaintNormalTile(specifiedTile, t, 50, UVRotateType._90);

        specifiedTile = GetATile(tile, 2, -1);
        PaintNormalTile(specifiedTile, t, 52, UVRotateType._90);

        specifiedTile = GetATile(tile, -3, 0);
        PaintNormalTile(specifiedTile, t, 51, UVRotateType._90);

        specifiedTile = GetATile(tile, -1, 0);
        PaintNormalTile(specifiedTile, t, 52, UVRotateType._90);

        specifiedTile = GetATile(tile, -1, 2);
        PaintNormalTile(specifiedTile, t, 54, UVRotateType._90);
        specifiedTile = GetATile(tile, 0, 2);
        PaintNormalTile(specifiedTile, t, 55, UVRotateType._90);
        specifiedTile = GetATile(tile, -1, 1);
        PaintNormalTile(specifiedTile, t, 62, UVRotateType._90);
        specifiedTile = GetATile(tile, 0, 1);
        PaintNormalTile(specifiedTile, t, 63, UVRotateType._90);

        specifiedTile = GetATile(tile, 4, 1);
        PaintNormalTile(specifiedTile, t, 51, UVRotateType._90);

        specifiedTile = GetATile(tile, 2, 2);
        PaintNormalTile(specifiedTile, t, 49, UVRotateType._90);
        specifiedTile = GetATile(tile, 3, 2);
        PaintNormalTile(specifiedTile, t, 50, UVRotateType._90);

        specifiedTile = GetATile(tile, -1, 3);
        PaintNormalTile(specifiedTile, t, 51, UVRotateType._90);

        specifiedTile = GetATile(tile, 1, 4);
        PaintNormalTile(specifiedTile, t, 52, UVRotateType._90);
    }

    public void PaintCrystalLevel3(PATile tile, int t)
    {
        PATile[] line1Tiles = GetLineTiles(tile.x, tile.y, -2, -12, 6);
        CalcTileBits(t, line1Tiles[0], 2);
        CalcTileBits(t, line1Tiles[1], 6);
        CalcTileBits(t, line1Tiles[2], 6);
        CalcTileBits(t, line1Tiles[3], 6);
        CalcTileBits(t, line1Tiles[4], 6);
        CalcTileBits(t, line1Tiles[5], 4);

        PATile[] line2Tiles = GetLineTiles(tile.x, tile.y, -5, -11, 12);
        CalcTileBits(t, line2Tiles[0], 2);
        CalcTileBits(t, line2Tiles[1], 6);
        CalcTileBits(t, line2Tiles[2], 6);
        CalcTileBits(t, line2Tiles[3], 7);
        PaintNormalTiles(line2Tiles, t, 4, 4);
        CalcTileBits(t, line2Tiles[8], 14);
        CalcTileBits(t, line2Tiles[9], 6);
        CalcTileBits(t, line2Tiles[10], 6);
        CalcTileBits(t, line2Tiles[11], 4);

        PATile[] line3Tiles = GetLineTiles(tile.x, tile.y, -6, -10, 14);
        CalcTileBits(t, line3Tiles[0], 2);
        CalcTileBits(t, line3Tiles[1], 7);
        PaintNormalTiles(line3Tiles, t, 2, 10);
        CalcTileBits(t, line3Tiles[12], 14);
        CalcTileBits(t, line3Tiles[13], 4);

        PATile[] line4Tiles = GetLineTiles(tile.x, tile.y, -8, -9, 18);
        CalcTileBits(t, line4Tiles[0], 2);
        CalcTileBits(t, line4Tiles[1], 6);
        CalcTileBits(t, line4Tiles[2], 7);
        PaintNormalTiles(line4Tiles, t, 3, 12);
        CalcTileBits(t, line4Tiles[15], 14);
        CalcTileBits(t, line4Tiles[16], 6);
        CalcTileBits(t, line4Tiles[17], 4);

        PATile[] line5Tiles = GetLineTiles(tile.x, tile.y, -9, -8, 20);
        CalcTileBits(t, line5Tiles[0], 2);
        CalcTileBits(t, line5Tiles[1], 7);
        PaintNormalTiles(line5Tiles, t, 2, 16);
        CalcTileBits(t, line5Tiles[18], 14);
        CalcTileBits(t, line5Tiles[19], 4);

        PATile[] line6Tiles = GetLineTiles(tile.x, tile.y, -9, -7, 20);
        CalcTileBits(t, line6Tiles[0], 3);
        PaintNormalTiles(line6Tiles, t, 1, 18);
        CalcTileBits(t, line6Tiles[19], 12);

        PATile[] line7Tiles = GetLineTiles(tile.x, tile.y, -10, -6, 22);
        CalcTileBits(t, line7Tiles[0], 2);
        CalcTileBits(t, line7Tiles[1], 7);
        PaintNormalTiles(line7Tiles, t, 2, 18);
        CalcTileBits(t, line7Tiles[20], 14);
        CalcTileBits(t, line7Tiles[21], 4);

        PATile[] line8Tiles = GetLineTiles(tile.x, tile.y, -11, -5, 24);
        CalcTileBits(t, line8Tiles[0], 2);
        CalcTileBits(t, line8Tiles[1], 7);
        PaintNormalTiles(line8Tiles, t, 2, 20);
        CalcTileBits(t, line8Tiles[22], 14);
        CalcTileBits(t, line8Tiles[23], 4);

        PATile[] line9Tiles = GetLineTiles(tile.x, tile.y, -11, -4, 24);
        CalcTileBits(t, line9Tiles[0], 3);
        PaintNormalTiles(line9Tiles, t, 1, 22);
        CalcTileBits(t, line9Tiles[23], 12);

        PATile[] line10Tiles = GetLineTiles(tile.x, tile.y, -11, -3, 24);
        CalcTileBits(t, line10Tiles[0], 3);
        PaintNormalTiles(line10Tiles, t, 1, 22);
        CalcTileBits(t, line10Tiles[23], 12);

        PATile[] line11Tiles = GetLineTiles(tile.x, tile.y, -12, -2, 26);
        CalcTileBits(t, line11Tiles[0], 2);
        CalcTileBits(t, line11Tiles[1], 7);
        PaintNormalTiles(line11Tiles, t, 2, 22);
        CalcTileBits(t, line11Tiles[24], 14);
        CalcTileBits(t, line11Tiles[25], 4);

        PATile[] line12Tiles = GetLineTiles(tile.x, tile.y, -12, -1, 26);
        CalcTileBits(t, line12Tiles[0], 3);
        PaintNormalTiles(line12Tiles, t, 1, 24);
        CalcTileBits(t, line12Tiles[25], 12);

        PATile[] line13Tiles = GetLineTiles(tile.x, tile.y, -12, 0, 26);
        CalcTileBits(t, line13Tiles[0], 3);
        PaintNormalTiles(line13Tiles, t, 1, 24);
        CalcTileBits(t, line13Tiles[25], 12);

        PATile[] line14Tiles = GetLineTiles(tile.x, tile.y, -12, 1, 26);
        CalcTileBits(t, line14Tiles[0], 3);
        PaintNormalTiles(line14Tiles, t, 1, 24);
        CalcTileBits(t, line14Tiles[25], 12);

        PATile[] line15Tiles = GetLineTiles(tile.x, tile.y, -12, 2, 26);
        CalcTileBits(t, line15Tiles[0], 3);
        PaintNormalTiles(line15Tiles, t, 1, 24);
        CalcTileBits(t, line15Tiles[25], 12);

        PATile[] line16Tiles = GetLineTiles(tile.x, tile.y, -12, 3, 26);
        CalcTileBits(t, line16Tiles[0], 1);
        CalcTileBits(t, line16Tiles[1], 11);
        PaintNormalTiles(line16Tiles, t, 2, 22);
        CalcTileBits(t, line16Tiles[24], 13);
        CalcTileBits(t, line16Tiles[25], 8);

        PATile[] line17Tiles = GetLineTiles(tile.x, tile.y, -11, 4, 24);
        CalcTileBits(t, line17Tiles[0], 3);
        PaintNormalTiles(line17Tiles, t, 1, 22);
        CalcTileBits(t, line17Tiles[23], 12);

        PATile[] line18Tiles = GetLineTiles(tile.x, tile.y, -11, 5, 24);
        CalcTileBits(t, line18Tiles[0], 3);
        PaintNormalTiles(line18Tiles, t, 1, 22);
        CalcTileBits(t, line18Tiles[23], 12);

        PATile[] line19Tiles = GetLineTiles(tile.x, tile.y, -11, 6, 24);
        CalcTileBits(t, line19Tiles[0], 1);
        CalcTileBits(t, line19Tiles[1], 11);
        PaintNormalTiles(line19Tiles, t, 2, 20);
        CalcTileBits(t, line19Tiles[22], 13);
        CalcTileBits(t, line19Tiles[23], 8);

        PATile[] line20Tiles = GetLineTiles(tile.x, tile.y, -10, 7, 22);
        CalcTileBits(t, line20Tiles[0], 1);
        CalcTileBits(t, line20Tiles[1], 11);
        PaintNormalTiles(line20Tiles, t, 2, 18);
        CalcTileBits(t, line20Tiles[20], 13);
        CalcTileBits(t, line20Tiles[21], 8);

        PATile[] line21Tiles = GetLineTiles(tile.x, tile.y, -9, 8, 20);
        CalcTileBits(t, line21Tiles[0], 3);
        PaintNormalTiles(line21Tiles, t, 1, 18);
        CalcTileBits(t, line21Tiles[19], 12);

        PATile[] line22Tiles = GetLineTiles(tile.x, tile.y, -9, 9, 20);
        CalcTileBits(t, line22Tiles[0], 1);
        CalcTileBits(t, line22Tiles[1], 11);
        PaintNormalTiles(line22Tiles, t, 2, 16);
        CalcTileBits(t, line22Tiles[18], 13);
        CalcTileBits(t, line22Tiles[19], 8);

        PATile[] line23Tiles = GetLineTiles(tile.x, tile.y, -8, 10, 18);
        CalcTileBits(t, line23Tiles[0], 1);
        CalcTileBits(t, line23Tiles[1], 9);
        CalcTileBits(t, line23Tiles[2], 11);
        PaintNormalTiles(line23Tiles, t, 3, 12);
        CalcTileBits(t, line23Tiles[15], 13);
        CalcTileBits(t, line23Tiles[16], 9);
        CalcTileBits(t, line23Tiles[17], 8);

        PATile[] line24Tiles = GetLineTiles(tile.x, tile.y, -6, 11, 14);
        CalcTileBits(t, line24Tiles[0], 1);
        CalcTileBits(t, line24Tiles[1], 11);
        PaintNormalTiles(line24Tiles, t, 2, 10);
        CalcTileBits(t, line24Tiles[12], 13);
        CalcTileBits(t, line24Tiles[13], 8);

        PATile[] line25Tiles = GetLineTiles(tile.x, tile.y, -5, 12, 12);
        CalcTileBits(t, line25Tiles[0], 1);
        CalcTileBits(t, line25Tiles[1], 9);
        CalcTileBits(t, line25Tiles[2], 9);
        CalcTileBits(t, line25Tiles[3], 11);
        PaintNormalTiles(line25Tiles, t, 4, 4);
        CalcTileBits(t, line25Tiles[8], 13);
        CalcTileBits(t, line25Tiles[9], 9);
        CalcTileBits(t, line25Tiles[10], 9);
        CalcTileBits(t, line25Tiles[11], 8);

        PATile[] line26Tiles = GetLineTiles(tile.x, tile.y, -2, 13, 6);
        CalcTileBits(t, line26Tiles[0], 1);
        CalcTileBits(t, line26Tiles[1], 9);
        CalcTileBits(t, line26Tiles[2], 9);
        CalcTileBits(t, line26Tiles[3], 9);
        CalcTileBits(t, line26Tiles[4], 9);
        CalcTileBits(t, line26Tiles[5], 8);
    }

    public void PaintCrystalLevel3_C(PATile tile, int t)
    {
        PaintALine(tile, -2, -4, 6, t, new byte[] { 2, 6 ,6});
        PaintALine(tile, -3, -3, 8, t, new byte[] { 2, 7 });
        PaintALine(tile, -4, -2, 10, t, new byte[] { 2, 7 });
        PaintALine(tile, -4, -1, 10, t, new byte[] { 3 });
        PaintALine(tile, -4, 0, 10, t, new byte[] { 3 });
    }

    public void PaintCrystalLevel3_B(PATile tile, int t)
    {
        PaintALine(tile, -1, -11, 4, t, new byte[] { 2, 6 });
        PaintALine(tile, -2, -10, 6, t, new byte[] { 2, 7 });
        PaintALine(tile, -4, -9, 10, t, new byte[] { 2, 6, 7 });
        PaintALine(tile, -6, -8, 14, t, new byte[] { 2, 6, 7 });
        PaintALine(tile, -7, -7, 16, t, new byte[] { 2, 7 });
        PaintALine(tile, -7, -6, 16, t, new byte[] { 3 });
        PaintALine(tile, -8, -5, 18, t, new byte[] { 2, 7 });
        PaintALine(tile, -9, -4, 20, t, new byte[] { 2, 7 });
        PaintALine(tile, -9, -3, 20, t, new byte[] { 3 });
        PaintALine(tile, -10, -2, 22, t, new byte[] { 2, 7 });
        PaintALine(tile, -11, -1, 24, t, new byte[] { 2, 7 });
        PaintALine(tile, -11, 0, 24, t, new byte[] { 3 });

        PATile specifiedTile = GetATile(tile,-1,-5);
        PaintNormalTile(specifiedTile,t,52,UVRotateType._270);

        specifiedTile = GetATile(tile, 2, -5);
        PaintNormalTile(specifiedTile, t, 49, UVRotateType._90);
        specifiedTile = GetATile(tile, 3, -5);
        PaintNormalTile(specifiedTile, t, 50, UVRotateType._90);

        specifiedTile = GetATile(tile, 4, -5);
        PaintNormalTile(specifiedTile, t, 52, UVRotateType._90);

        specifiedTile = GetATile(tile, -5, -3);
        PaintNormalTile(specifiedTile, t, 54, UVRotateType._90);
        specifiedTile = GetATile(tile, -4, -3);
        PaintNormalTile(specifiedTile, t, 55, UVRotateType._90);
        specifiedTile = GetATile(tile, -5, -4);
        PaintNormalTile(specifiedTile, t, 62, UVRotateType._90);
        specifiedTile = GetATile(tile, -4, -4);
        PaintNormalTile(specifiedTile, t, 63, UVRotateType._90);

        specifiedTile = GetATile(tile, -3, -4);
        PaintNormalTile(specifiedTile, t, 51, UVRotateType._90);

        specifiedTile = GetATile(tile, 6, -1);
        PaintNormalTile(specifiedTile, t, 52, UVRotateType.None);

        specifiedTile = GetATile(tile, 6, 3);
        PaintNormalTile(specifiedTile, t, 55, UVRotateType.None);
        specifiedTile = GetATile(tile, 7, 3);
        PaintNormalTile(specifiedTile, t, 63, UVRotateType.None);
        specifiedTile = GetATile(tile, 6, 2);
        PaintNormalTile(specifiedTile, t, 54, UVRotateType.None);
        specifiedTile = GetATile(tile, 7, 2);
        PaintNormalTile(specifiedTile, t, 62, UVRotateType.None);

        specifiedTile = GetATile(tile, -4, 6);
        PaintNormalTile(specifiedTile, t, 55, UVRotateType.None);
        specifiedTile = GetATile(tile, -3, 6);
        PaintNormalTile(specifiedTile, t, 63, UVRotateType.None);
        specifiedTile = GetATile(tile, -4, 5);
        PaintNormalTile(specifiedTile, t, 54, UVRotateType.None);
        specifiedTile = GetATile(tile, -3, 5);
        PaintNormalTile(specifiedTile, t, 62, UVRotateType.None);

        specifiedTile = GetATile(tile, -1, 6);
        PaintNormalTile(specifiedTile, t, 51, UVRotateType._90);

        specifiedTile = GetATile(tile, 2, 6);
        PaintNormalTile(specifiedTile, t, 49, UVRotateType._90);
        specifiedTile = GetATile(tile, 3, 6);
        PaintNormalTile(specifiedTile, t, 50, UVRotateType._90);
    }

    public void PaintTileElementLevel1(PATile tile,TileElementType elementType,bool isReset = false)
    {
        PATile[] line1Tiles = GetLineTiles(tile.x, tile.y, -1, -4, 4);
        SetTilesElement(line1Tiles,0,4,elementType,1,isReset);
        
        PATile[] line2Tiles = GetLineTiles(tile.x, tile.y, -2, -3, 6);
        SetTilesElement(line2Tiles, 0,6,elementType, 1,isReset);
        
        PATile[] line3Tiles = GetLineTiles(tile.x, tile.y, -3, -2, 8);
        SetTilesElement(line3Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line3Tiles, 2, 4, elementType, 2, isReset);
        SetTilesElement(line3Tiles, 6, 2, elementType, 1, isReset);
        
        PATile[] line4Tiles = GetLineTiles(tile.x, tile.y, -4, -1, 10);
        SetTilesElement(line4Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line4Tiles, 2, 6, elementType, 2, isReset);
        SetTilesElement(line4Tiles, 8, 2, elementType, 1, isReset);
       
        PATile[] line5Tiles = GetLineTiles(tile.x, tile.y, -4, 0, 10);
        SetTilesElement(line5Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line5Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElement(line5Tiles, 6, 2, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 8, 2, elementType, 1, isReset);
        
        PATile[] line6Tiles = GetLineTiles(tile.x, tile.y, -4, 1, 10);
        SetTilesElement(line6Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line6Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElement(line6Tiles, 6, 2, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 8, 2, elementType, 1, isReset);

        PATile[] line7Tiles = GetLineTiles(tile.x, tile.y, -4, 2, 10);
        SetTilesElement(line7Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line7Tiles, 2, 6, elementType, 2, isReset);
        SetTilesElement(line7Tiles, 8, 2, elementType, 1, isReset);

        PATile[] line8Tiles = GetLineTiles(tile.x, tile.y, -3, 3, 8);
        SetTilesElement(line8Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line8Tiles, 2, 4, elementType, 2, isReset);
        SetTilesElement(line8Tiles, 6, 2, elementType, 1, isReset);

        PATile[] line9Tiles = GetLineTiles(tile.x, tile.y, -2, 4, 6);
        SetTilesElement(line9Tiles, 0, 6, elementType, 1, isReset);

        PATile[] line10Tiles = GetLineTiles(tile.x, tile.y, -1, 5, 4);
        SetTilesElement(line10Tiles, 0, 4, elementType, 1, isReset);
    }

    public void PaintTileElementLevel2(PATile tile, TileElementType elementType, bool isReset = false)
    {
        PATile[] line1Tiles = GetLineTiles(tile.x, tile.y, -2, -8, 6);
        SetTilesElement(line1Tiles, 0, 6, elementType, 1, isReset);

        PATile[] line2Tiles = GetLineTiles(tile.x, tile.y, -4, -7, 10);
        SetTilesElement(line2Tiles, 0, 10, elementType, 1, isReset);

        PATile[] line3Tiles = GetLineTiles(tile.x, tile.y, -5, -6, 12);
        SetTilesElement(line3Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line3Tiles, 3, 6, elementType, 2, isReset);
        SetTilesElement(line3Tiles, 9, 3, elementType, 1, isReset);

        PATile[] line4Tiles = GetLineTiles(tile.x, tile.y, -6, -5, 14);
        SetTilesElement(line4Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line4Tiles, 3, 8, elementType, 2, isReset);
        SetTilesElement(line4Tiles, 11, 3, elementType, 1, isReset);

        PATile[] line5Tiles = GetLineTiles(tile.x, tile.y, -7, -4, 16);
        SetTilesElement(line5Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line5Tiles, 3, 3, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 6, 4, elementType, 3, isReset);
        SetTilesElement(line5Tiles, 10, 3, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 13, 3, elementType, 1, isReset);

        PATile[] line6Tiles = GetLineTiles(tile.x, tile.y, -7, -3, 16);
        SetTilesElement(line6Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line6Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 5, 6, elementType, 3, isReset);
        SetTilesElement(line6Tiles, 11, 3, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 14, 2, elementType, 1, isReset);

        PATile[] line7Tiles = GetLineTiles(tile.x, tile.y, -8, -2, 18);
        SetTilesElement(line7Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line7Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line7Tiles, 5, 2, elementType, 3, isReset);
        SetTilesElement(line7Tiles, 7, 4, elementType, 4, isReset);
        SetTilesElement(line7Tiles, 11, 2, elementType, 3, isReset);
        SetTilesElement(line7Tiles, 13, 3, elementType, 2, isReset);
        SetTilesElement(line7Tiles, 16, 2, elementType, 1, isReset);

        PATile[] line8Tiles = GetLineTiles(tile.x, tile.y, -8, -1, 18);
        SetTilesElement(line8Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line8Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElement(line8Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElement(line8Tiles, 6, 6, elementType, 4, isReset);
        SetTilesElement(line8Tiles, 12, 2, elementType, 3, isReset);
        SetTilesElement(line8Tiles, 14, 2, elementType, 2, isReset);
        SetTilesElement(line8Tiles, 16, 2, elementType, 1, isReset);

        PATile[] line9Tiles = GetLineTiles(tile.x, tile.y, -8, 0, 18);
        SetTilesElement(line9Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line9Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElement(line9Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElement(line9Tiles, 6, 2, elementType, 4, isReset);
        SetTilesElement(line9Tiles, 8, 2, elementType, 5, isReset);
        SetTilesElement(line9Tiles, 10, 2, elementType, 4, isReset);
        SetTilesElement(line9Tiles, 12, 2, elementType, 3, isReset);
        SetTilesElement(line9Tiles, 14, 2, elementType, 2, isReset);
        SetTilesElement(line9Tiles, 16, 2, elementType, 1, isReset);

        PATile[] line10Tiles = GetLineTiles(tile.x, tile.y, -8, 1, 18);
        SetTilesElement(line10Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line10Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElement(line10Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElement(line10Tiles, 6, 2, elementType, 4, isReset);
        SetTilesElement(line10Tiles, 8, 2, elementType, 5, isReset);
        SetTilesElement(line10Tiles, 10, 2, elementType, 4, isReset);
        SetTilesElement(line10Tiles, 12, 2, elementType, 3, isReset);
        SetTilesElement(line10Tiles, 14, 2, elementType, 2, isReset);
        SetTilesElement(line10Tiles, 16, 2, elementType, 1, isReset);

        PATile[] line11Tiles = GetLineTiles(tile.x, tile.y, -8, 2, 18);
        SetTilesElement(line11Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line11Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElement(line11Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElement(line11Tiles, 6, 6, elementType, 4, isReset);
        SetTilesElement(line11Tiles, 12, 2, elementType, 3, isReset);
        SetTilesElement(line11Tiles, 14, 2, elementType, 2, isReset);
        SetTilesElement(line11Tiles, 16, 2, elementType, 1, isReset);

        PATile[] line12Tiles = GetLineTiles(tile.x, tile.y, -8, 3, 18);
        SetTilesElement(line12Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line12Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line12Tiles, 5, 2, elementType, 3, isReset);
        SetTilesElement(line12Tiles, 7, 4, elementType, 4, isReset);
        SetTilesElement(line12Tiles, 11, 2, elementType, 3, isReset);
        SetTilesElement(line12Tiles, 13, 3, elementType, 2, isReset);
        SetTilesElement(line12Tiles, 16, 2, elementType, 1, isReset);

        PATile[] line13Tiles = GetLineTiles(tile.x, tile.y, -7, 4, 16);
        SetTilesElement(line13Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line13Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line13Tiles, 5, 6, elementType, 3, isReset);
        SetTilesElement(line13Tiles, 11, 3, elementType, 2, isReset);
        SetTilesElement(line13Tiles, 14, 2, elementType, 1, isReset);

        PATile[] line14Tiles = GetLineTiles(tile.x, tile.y, -7, 5, 16);
        SetTilesElement(line14Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line14Tiles, 3, 3, elementType, 2, isReset);
        SetTilesElement(line14Tiles, 6, 4, elementType, 3, isReset);
        SetTilesElement(line14Tiles, 10, 3, elementType, 2, isReset);
        SetTilesElement(line14Tiles, 13, 3, elementType, 1, isReset);

        PATile[] line15Tiles = GetLineTiles(tile.x, tile.y, -6, 6, 14);
        SetTilesElement(line15Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line15Tiles, 3, 8, elementType, 2, isReset);
        SetTilesElement(line15Tiles, 11, 3, elementType, 1, isReset);

        PATile[] line16Tiles = GetLineTiles(tile.x, tile.y, -5, 7, 12);
        SetTilesElement(line16Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line16Tiles, 3, 6, elementType, 2, isReset);
        SetTilesElement(line16Tiles, 9, 3, elementType, 1, isReset);

        PATile[] line17Tiles = GetLineTiles(tile.x, tile.y, -4, 8, 10);
        SetTilesElement(line17Tiles, 0, 10, elementType, 1, isReset);
        
        PATile[] line18Tiles = GetLineTiles(tile.x, tile.y, -2, 9, 6);
        SetTilesElement(line18Tiles, 0, 6, elementType, 1, isReset);
    }

    public void PaintTileElementLevel3(PATile tile, TileElementType elementType, bool isReset = false)
    {
        PATile[] line1Tiles = GetLineTiles(tile.x, tile.y, -2, -12, 6);
        SetTilesElement(line1Tiles, 0, 6, elementType, 1, isReset);

        line1Tiles = GetLineTiles(tile.x, tile.y, -2, 13, 6);
        SetTilesElement(line1Tiles, 0, 6, elementType, 1, isReset);

        PATile[] line2Tiles = GetLineTiles(tile.x, tile.y, -5, -11, 12);
        SetTilesElement(line2Tiles, 0, 12, elementType, 1, isReset);

        line2Tiles = GetLineTiles(tile.x, tile.y, -5, 12, 12);
        SetTilesElement(line2Tiles, 0, 12, elementType, 1, isReset);

        PATile[] line3Tiles = GetLineTiles(tile.x, tile.y, -6, -10, 14);
        SetTilesElement(line3Tiles, 0, 4, elementType, 1, isReset);
        SetTilesElement(line3Tiles, 4, 6, elementType, 2, isReset);
        SetTilesElement(line3Tiles, 10, 4, elementType, 1, isReset);

        line3Tiles = GetLineTiles(tile.x, tile.y, -6, 11, 14);
        SetTilesElement(line3Tiles, 0, 4, elementType, 1, isReset);
        SetTilesElement(line3Tiles, 4, 6, elementType, 2, isReset);
        SetTilesElement(line3Tiles, 10, 4, elementType, 1, isReset);

        PATile[] line4Tiles = GetLineTiles(tile.x, tile.y, -8, -9, 18);
        SetTilesElement(line4Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line4Tiles, 3, 12, elementType, 2, isReset);
        SetTilesElement(line4Tiles, 15, 3, elementType, 1, isReset);

        line4Tiles = GetLineTiles(tile.x, tile.y, -8, 10, 18);
        SetTilesElement(line4Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line4Tiles, 3, 12, elementType, 2, isReset);
        SetTilesElement(line4Tiles, 15, 3, elementType, 1, isReset);

        PATile[] line5Tiles = GetLineTiles(tile.x, tile.y, -9, -8, 20);
        SetTilesElement(line5Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line5Tiles, 3, 4, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 7, 6, elementType, 3, isReset);
        SetTilesElement(line5Tiles, 13, 4, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 17, 3, elementType, 1, isReset);

        line5Tiles = GetLineTiles(tile.x, tile.y, -9, 9, 20);
        SetTilesElement(line5Tiles, 0, 3, elementType, 1, isReset);
        SetTilesElement(line5Tiles, 3, 4, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 7, 6, elementType, 3, isReset);
        SetTilesElement(line5Tiles, 13, 4, elementType, 2, isReset);
        SetTilesElement(line5Tiles, 17, 3, elementType, 1, isReset);

        PATile[] line6Tiles = GetLineTiles(tile.x, tile.y, -9, -7, 20);
        SetTilesElement(line6Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line6Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 5, 10, elementType, 3, isReset);
        SetTilesElement(line6Tiles, 15, 3, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 18, 2, elementType, 1, isReset);

        line6Tiles = GetLineTiles(tile.x, tile.y, -9, 8, 20);
        SetTilesElement(line6Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line6Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 5, 10, elementType, 3, isReset);
        SetTilesElement(line6Tiles, 15, 3, elementType, 2, isReset);
        SetTilesElement(line6Tiles, 18, 2, elementType, 1, isReset);

        PATile[] line7Tiles = GetLineTiles(tile.x, tile.y, -10, -6, 22);
        SetTilesElement(line7Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line7Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line7Tiles, 5, 3, elementType, 3, isReset);
        SetTilesElement(line7Tiles, 8, 6, elementType, 4, isReset);
        SetTilesElement(line7Tiles, 14, 3, elementType, 3, isReset);
        SetTilesElement(line7Tiles, 17, 3, elementType, 2, isReset);
        SetTilesElement(line7Tiles, 20, 2, elementType, 1, isReset);

        line7Tiles = GetLineTiles(tile.x, tile.y, -10, 7, 22);
        SetTilesElement(line7Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElement(line7Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElement(line7Tiles, 5, 3, elementType, 3, isReset);
        SetTilesElement(line7Tiles, 8, 6, elementType, 4, isReset);
        SetTilesElement(line7Tiles, 14, 3, elementType, 3, isReset);
        SetTilesElement(line7Tiles, 17, 3, elementType, 2, isReset);
        SetTilesElement(line7Tiles, 20, 2, elementType, 1, isReset);

        PATile[] line8Tiles = GetLineTiles(tile.x, tile.y, -11, -5, 24);
        SetTilesElementSym(line8Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line8Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElementSym(line8Tiles, 5, 3, elementType, 3, isReset);
        SetTilesElementSym(line8Tiles, 8, 4, elementType, 4, isReset);

        GetLineTiles(tile.x, tile.y, -11, 6, 24);
        SetTilesElementSym(line8Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line8Tiles, 2, 3, elementType, 2, isReset);
        SetTilesElementSym(line8Tiles, 5, 3, elementType, 3, isReset);
        SetTilesElementSym(line8Tiles, 8, 4, elementType, 4, isReset);

        PATile[] line9Tiles = GetLineTiles(tile.x, tile.y, -11, -4, 24);
        SetTilesElementSym(line9Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line9Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line9Tiles, 4, 3, elementType, 3, isReset);
        SetTilesElementSym(line9Tiles, 7, 3, elementType, 4, isReset);
        SetTilesElementSym(line9Tiles, 10, 2, elementType, 5, isReset);

        line9Tiles = GetLineTiles(tile.x, tile.y, -11, 5, 24);
        SetTilesElementSym(line9Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line9Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line9Tiles, 4, 3, elementType, 3, isReset);
        SetTilesElementSym(line9Tiles, 7, 3, elementType, 4, isReset);
        SetTilesElementSym(line9Tiles, 10, 2, elementType, 5, isReset);

        PATile[] line10Tiles = GetLineTiles(tile.x, tile.y, -11, -3, 24);
        SetTilesElementSym(line10Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line10Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line10Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line10Tiles, 6, 3, elementType, 4, isReset);
        SetTilesElementSym(line10Tiles, 9, 3, elementType, 5, isReset);

        line10Tiles = GetLineTiles(tile.x, tile.y, -11, 4, 24);
        SetTilesElementSym(line10Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line10Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line10Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line10Tiles, 6, 3, elementType, 4, isReset);
        SetTilesElementSym(line10Tiles, 9, 3, elementType, 5, isReset);

        PATile[] line11Tiles = GetLineTiles(tile.x, tile.y, -12, -2, 26);
        SetTilesElementSym(line11Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line11Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line11Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line11Tiles, 6, 3, elementType, 4, isReset);
        SetTilesElementSym(line11Tiles, 9, 2, elementType, 5, isReset);
        SetTilesElementSym(line11Tiles, 11, 2, elementType, 6, isReset);

        line11Tiles = GetLineTiles(tile.x, tile.y, -12, 3, 26);
        SetTilesElementSym(line11Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line11Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line11Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line11Tiles, 6, 3, elementType, 4, isReset);
        SetTilesElementSym(line11Tiles, 9, 2, elementType, 5, isReset);
        SetTilesElementSym(line11Tiles, 11, 2, elementType, 6, isReset);

        PATile[] line12Tiles = GetLineTiles(tile.x, tile.y, -12, -1, 26);
        SetTilesElementSym(line12Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line12Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line12Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line12Tiles, 6, 2, elementType, 4, isReset);
        SetTilesElementSym(line12Tiles, 8, 2, elementType, 5, isReset);
        SetTilesElementSym(line12Tiles, 10, 3, elementType, 6, isReset);

        line12Tiles = GetLineTiles(tile.x, tile.y, -12, 2, 26);
        SetTilesElementSym(line12Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line12Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line12Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line12Tiles, 6, 2, elementType, 4, isReset);
        SetTilesElementSym(line12Tiles, 8, 2, elementType, 5, isReset);
        SetTilesElementSym(line12Tiles, 10, 3, elementType, 6, isReset);

        PATile[] line13Tiles = GetLineTiles(tile.x, tile.y, -12, 0, 26);
        SetTilesElementSym(line13Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line13Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line13Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line13Tiles, 6, 2, elementType, 4, isReset);
        SetTilesElementSym(line13Tiles, 8, 2, elementType, 5, isReset);
        SetTilesElementSym(line13Tiles, 10, 2, elementType, 6, isReset);
        SetTilesElementSym(line13Tiles, 12, 1, elementType, 7, isReset);

        line13Tiles = GetLineTiles(tile.x, tile.y, -12, 1, 26);
        SetTilesElementSym(line13Tiles, 0, 2, elementType, 1, isReset);
        SetTilesElementSym(line13Tiles, 2, 2, elementType, 2, isReset);
        SetTilesElementSym(line13Tiles, 4, 2, elementType, 3, isReset);
        SetTilesElementSym(line13Tiles, 6, 2, elementType, 4, isReset);
        SetTilesElementSym(line13Tiles, 8, 2, elementType, 5, isReset);
        SetTilesElementSym(line13Tiles, 10, 2, elementType, 6, isReset);
        SetTilesElementSym(line13Tiles, 12, 1, elementType, 7, isReset);
    }
}