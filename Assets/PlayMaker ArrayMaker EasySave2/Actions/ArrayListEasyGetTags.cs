//	(c) Jean Fabre, 2011-2015 All rights reserved.
//	http://www.fabrejean.net

// INSTRUCTIONS
// Drop a PlayMakerArrayList script onto a GameObject, and define a unique name for reference if several PlayMakerArrayList coexists on that GameObject.
// In this Action interface, link that GameObject in "arrayListObject" and input the reference name if defined. 
// Note: You can directly reference that GameObject or store it in an Fsm variable or global Fsm variable

using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Easy Save 2")]
	[Tooltip("Loads into a PlayMaker Array List Proxy component the tags of an EasySave file")]
	public class ArrayListEasyGetTags : ArrayListActions
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

		[RequiredField]
		[Tooltip("The name or absolute path of the file where our data will be stored. If the file doesn't exist, it will be created.")]
		public FsmString filename = "defaultFile.txt";

		[Tooltip("Whether the data is encrypted or not. If set to true, you must set an encryption password.")]
		public FsmBool encrypt = ES2GlobalSettings.defaultEncrypt;
		[Tooltip("The password to use for encryption if it is enabled.")]
		public FsmString encryptionPassword = ES2GlobalSettings.defaultEncryptionPassword;

		[ActionSection("Result")]
		[Tooltip("The number of tags found on that file.")]
		[UIHint(UIHint.Variable)]
		public FsmInt tagCount;

		[Tooltip("The event to send if accessing tag was successful.")]
		public FsmEvent isDoneEvent;
		[Tooltip("The event to send if no tag were found on the file.")]
		public FsmEvent noTagsEvent;
		[Tooltip("The event to send if there was an error.")]
		public FsmEvent isErrorEvent;

		[Tooltip("Where any errors thrown will be stored. Set this to a variable, or leave it blank.")]
		[UIHint(UIHint.Variable)]
		public FsmString errorMessage = "";

		public override void Reset()
		{
			gameObject = null;
			reference = null;
		
			filename.Value = "defaultFile.txt";
			encrypt.Value = ES2GlobalSettings.defaultEncrypt;
			encryptionPassword.Value = ES2GlobalSettings.defaultEncryptionPassword;

			tagCount = null;
			isDoneEvent = null;
			noTagsEvent = null;
			isErrorEvent = null;
			errorMessage = new FsmString(){UseVariable=true};
		}
		
		public override void OnEnter()
		{

			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				GetTags();


			Finish();
		}
		
		
		public void GetTags()
		{
			if (! isProxyValid() ) 
				return;

			ES2Settings _setting =  new ES2Settings();
			_setting.encrypt = encrypt.Value;
			_setting.encryptionPassword = encryptionPassword.Value;

			proxy.arrayList.Clear();
			string[] tagsInFile = new string[0];

			try{

				tagsInFile = ES2.GetTags(filename.Value,_setting);

			}catch(Exception e)
			{
				errorMessage.Value = e.Message;
				Fsm.Event(isErrorEvent);
			}

			foreach(string element in tagsInFile){
				proxy.arrayList.Add(element);
			}

			tagCount.Value = tagsInFile.Length;
			if (tagsInFile.Length==0)
			{
				Fsm.Event(noTagsEvent);
			}

			Fsm.Event(isDoneEvent);
		}
		
		
	}
}