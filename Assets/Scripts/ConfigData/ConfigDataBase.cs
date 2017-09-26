using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Game.Messenger;
using Debug = Debugger;

public class ConfigDataBase : SingletonAppMonoBehaviour<ConfigDataBase>
{
	private const string ConfigsBundleName = "Configs.assetBundle";
	private List<string> configFileNames = new List<string>();
	public bool loadFinish = false;
	private AssetBundle assetBundle;
	private int readCount = 0;

	public void StartLoad()
	{
		InitConfig ();
		StartCoroutine ("LoadConfig");
	}
	
	IEnumerator LoadConfig ()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        string path = Application.dataPath + "\\..\\AssetBundles\\" + GetPlatformName() + "\\configasset";
#elif UNITY_IPHONE  
        string path = Application.dataPath +"/Raw"+"/AssetBundles/" + GetPlatformName() + "/configasset";  
#elif UNITY_ANDROID  
        string path = "jar:file://" + Application.dataPath + "!/assets/"+"AssetBundles/" + GetPlatformName() + "/configasset";  
#endif
        WWW loadWWW = new WWW(path);
        yield return loadWWW;
        assetBundle = loadWWW.assetBundle;
        loadWWW.Dispose();
        loadFinish = true;
	}
	
	string GetPlatformName()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return "Android";
#elif UNITY_IPHONE  
        return "IOS";
#elif UNITY_ANDROID  
        return "Android";
#endif
    }
	
	private void InitConfig()
	{
		configFileNames.Add ("DecalConfig");
		configFileNames.Add ("CrystalRangeConfig");
		configFileNames.Add ("TileBrushConfig");
	}
	private DecalConfigAsset decalConfigAsset;
	public DecalConfigAsset DecalConfigAsset
	{
		get
		{
			if(decalConfigAsset == null)
			{
				float startTime = Time.realtimeSinceStartup;
				ConfigAssetBase asset = assetBundle.LoadAsset("DecalConfig",typeof(ConfigAssetBase)) as ConfigAssetBase;
				asset.readList();
				decalConfigAsset = asset as DecalConfigAsset;
				readCount ++;
				if(readCount == configFileNames.Count)
				{
					assetBundle.Unload(false);
				}
				Debug.Log("Read Config DecalConfigAsset Cost Time " + (Time.realtimeSinceStartup - startTime));
			}
			return decalConfigAsset;
		}
	}
	private CrystalRangeConfigAsset crystalRangeConfigAsset;
	public CrystalRangeConfigAsset CrystalRangeConfigAsset
	{
		get
		{
			if(crystalRangeConfigAsset == null)
			{
				float startTime = Time.realtimeSinceStartup;
				ConfigAssetBase asset = assetBundle.LoadAsset("CrystalRangeConfig",typeof(ConfigAssetBase)) as ConfigAssetBase;
				asset.readList();
				crystalRangeConfigAsset = asset as CrystalRangeConfigAsset;
				readCount ++;
				if(readCount == configFileNames.Count)
				{
					assetBundle.Unload(false);
				}
				Debug.Log("Read Config CrystalRangeConfigAsset Cost Time " + (Time.realtimeSinceStartup - startTime));
			}
			return crystalRangeConfigAsset;
		}
	}
	private TileBrushConfigAsset tileBrushConfigAsset;
	public TileBrushConfigAsset TileBrushConfigAsset
	{
		get
		{
			if(tileBrushConfigAsset == null)
			{
				float startTime = Time.realtimeSinceStartup;
				ConfigAssetBase asset = assetBundle.LoadAsset("TileBrushConfig",typeof(ConfigAssetBase)) as ConfigAssetBase;
				asset.readList();
				tileBrushConfigAsset = asset as TileBrushConfigAsset;
				readCount ++;
				if(readCount == configFileNames.Count)
				{
					assetBundle.Unload(false);
				}
				Debug.Log("Read Config TileBrushConfigAsset Cost Time " + (Time.realtimeSinceStartup - startTime));
			}
			return tileBrushConfigAsset;
		}
	}
}