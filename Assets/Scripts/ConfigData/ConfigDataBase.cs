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
		 string path = Application.dataPath + "\\..\\AssetBundles\\" + GetPlatformName() + "\\configasset";
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
		configFileNames.Add ("CdConfig");
	}
	private CdConfigAsset cdConfigAsset;
	public CdConfigAsset CdConfigAsset
	{
		get
		{
			if(cdConfigAsset == null)
			{
				float startTime = Time.realtimeSinceStartup;
				ConfigAssetBase asset = assetBundle.LoadAsset("CdConfig",typeof(ConfigAssetBase)) as ConfigAssetBase;
				asset.readList();
				cdConfigAsset = asset as CdConfigAsset;
				readCount ++;
				if(readCount == configFileNames.Count)
				{
					assetBundle.Unload(false);
				}
				Debug.Log("Read Config CdConfigAsset Cost Time " + (Time.realtimeSinceStartup - startTime));
			}
			return cdConfigAsset;
		}
	}
}