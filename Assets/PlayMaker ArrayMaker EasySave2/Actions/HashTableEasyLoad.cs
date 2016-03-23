//	(c) Jean Fabre, 2011-2015 All rights reserved.
//	http://www.fabrejean.net

// INSTRUCTIONS
// Drop a PlayMakerHashTableProxy script onto a GameObject, and define a unique name for reference if several PlayMakerHashTableProxy coexists on that GameObject.
// In this Action interface, link that GameObject in "hashTableObject" and input the reference name if defined. 
// Note: You can directly reference that GameObject or store it in an Fsm variable or global Fsm variable

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easy Save 2")]
	[Tooltip("Loads a PlayMaker HashTable Proxy component using EasySave")]
	public class HashTableEasyLoad : HashTableActions
	{
		
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;
		
		[ActionSection("Easy Save Set Up")]
		
		[Tooltip("A unique tag for this save. For example, the object's name if no other objects use the same name. Leave to none or empty, to use the GameObject Name + Fsm Name + array Reference as tag.")]
		public FsmString uniqueTag = "";
		
		[RequiredField]
		[Tooltip("The name of the file that we'll create to store our data.")]
		public FsmString saveFile = "defaultES2File.txt";
		
		[Tooltip("Whether the data we are loading is stored in the Resources folder.")]
		public FsmBool loadFromResources;
		
		public override void Reset()
		{
			gameObject = null;
			reference = null;
			
			uniqueTag = new FsmString(){UseVariable=true};
			
			saveFile = "defaultES2File.txt";
			
			loadFromResources = false;
		}
		
		public override void OnEnter()
		{

			if (SetUpHashTableProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value))
				LoadHashTable();
			
			Finish();
		}
		
		
		public void LoadHashTable()
		{
			if (! isProxyValid() ) 
				return;
			
			string _tag = uniqueTag.Value;
			if (string.IsNullOrEmpty(_tag))
			{
				_tag = Fsm.GameObjectName+"/"+Fsm.Name+"/hashTable/"+reference;
			}
			
			ES2Settings loadSettings = new ES2Settings();
			if(loadFromResources.Value)
			{
				loadSettings.saveLocation = ES2Settings.SaveLocation.Resources;
			}
			
			
		 	Dictionary<string,string> _dict =	ES2.LoadDictionary<string,string>(saveFile.Value+"?tag="+_tag);
			
			Log("Loaded from "+saveFile.Value+"?tag="+_tag);

			proxy.hashTable.Clear();
			
			
			foreach(string key in _dict.Keys)
			{		
				proxy.hashTable[key] = PlayMakerUtils.ParseValueFromString(_dict[key]);
			}
			
			Finish();
		}
		
		
	}
}