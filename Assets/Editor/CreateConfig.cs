using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public class CreateConfig
{
	private const string BundleName = "Configs";

	public static void MenuCreateConfigAsset ()
	{
		CreateConfigAssetList ();
		System.GC.Collect ();
	}
	
	public static void CreateConfigAsset<T>() where T : ConfigAssetBase
	{
		ConfigAssetBase asset = ScriptableObject.CreateInstance<T>() as ConfigAssetBase;
		string configName = asset.GetConfigName ();
		string assetPathAndName = "Assets/WorkAssets/ConfigAsset/" + configName + ".asset";
		if (File.Exists (assetPathAndName)) 
			File.Delete(assetPathAndName);
		
		string filePath = Application.dataPath + "/WorkAssets/ConfigText/" + configName + ".csv";
		CsvStreamReader reader = new CsvStreamReader (filePath,System.Text.Encoding.UTF8);
		
		if(reader.RowCount < 1)
		{
			Debug.LogError("CreateConfigAsset " + configName + " reader.RowCount < 1");
			return;
		}
		asset.ReadFromCSV (reader);
		
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();	
	}
	
	private static void CreateConfigAssetList(){

		CreateConfigAsset<DecalConfigAsset> ();
		CreateConfigAsset<TileMixConfigAsset> ();
		CreateConfigAsset<CrystalRangeConfigAsset> ();
		CreateConfigAsset<TileBrushConfigAsset> ();
	}
}