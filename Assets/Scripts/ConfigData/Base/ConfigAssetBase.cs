using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
  
public abstract class ConfigAssetBase : ScriptableObject
{
	[System.Serializable]
	public abstract class ConfigBase
	{
		public int id;
		public abstract void parse(ArrayList cells);
	}
	


	public abstract string GetConfigName ();
	public abstract ConfigBase GetNewConfig ();
	public abstract void add (ConfigBase config);
	public abstract void readList ();


	public void ReadFromCSV(CsvStreamReader reader)
	{
		for(int i = 1 ; i < reader.RowCount+1 ; i++)
		{
			ConfigBase configItem = GetNewConfig();

            try
            {
                configItem.parse(reader[i]);
            }
            catch (FormatException e)
            {
                Debug.LogError("FormatException! ConfigName: " + GetConfigName() + " id: " + configItem.id + " row: " + i);
				throw e;
            }
			add(configItem);
//			configs.Add(configItem.id,configItem);
		}
	}


}