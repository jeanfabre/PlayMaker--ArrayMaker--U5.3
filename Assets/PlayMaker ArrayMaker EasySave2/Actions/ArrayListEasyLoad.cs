//	(c) Jean Fabre, 2011-2015 All rights reserved.
//	http://www.fabrejean.net

// INSTRUCTIONS
// Drop a PlayMakerArrayList script onto a GameObject, and define a unique name for reference if several PlayMakerArrayList coexists on that GameObject.
// In this Action interface, link that GameObject in "arrayListObject" and input the reference name if defined. 
// Note: You can directly reference that GameObject or store it in an Fsm variable or global Fsm variable

using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easy Save 2")]
	[Tooltip("Loads a PlayMaker Array List Proxy component using EasySave")]
	public class ArrayListEasyLoad : ArrayListActions
	{
		
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
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

			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				LoadArrayList();
			
			Finish();
		}
		
		
		public void LoadArrayList()
		{
			if (! isProxyValid() ) 
				return;
			
			string _tag = uniqueTag.Value;
			if (string.IsNullOrEmpty(_tag))
			{
				_tag = Fsm.GameObjectName+"/"+Fsm.Name+"/arraylist/"+reference.Value;
			}
			
			ES2Settings loadSettings = new ES2Settings();
			if(loadFromResources.Value)
			{
				loadSettings.saveLocation = ES2Settings.SaveLocation.Resources;
			}
			
			List<string> source = ES2.LoadList<string>(saveFile.Value+"?tag="+_tag);
			
			Log("Loaded from "+saveFile.Value+"?tag="+uniqueTag);

			proxy.arrayList.Clear();
			
			foreach(string element in source){
				proxy.arrayList.Add(PlayMakerUtils.ParseValueFromString(element));
			}
			
			Finish();
		}
		
		
	}
}