//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using ws.winx.platform;
using System.ComponentModel;
using ws.winx.devices;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;

namespace ws.winx.input 
{

    public static class InputManager
    {
       
		       
       
		private static InputCombination[] __inputCombinations;
		private static InputSettings __settings;//=new InputSettings();
		private static IHIDInterface __hidInterface;//=new ws.winx.platform.windows.WinHIDInterface();
        private static List<IDriver> __drivers;


        /// <summary>
        /// Edit Mode = true stops all keys quering and checks while gameplay. Useful when use open UI to map keys to states
        /// 
        /// </summary>
        public static bool EditMode = false;





	 static JoystickDevicesCollection _joysticks;


		internal static IDeviceCollection Devices
		{
			
			get {  if(_joysticks==null)  _joysticks = new JoystickDevicesCollection(); return _joysticks; }
			
		}
      

		internal static IHIDInterface hidInterface{
			get{ 
               
				if(__hidInterface==null){
					//if((Application.platform & (RuntimePlatform.WindowsPlayer | RuntimePlatform.WindowsEditor))!=0){

                        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                        __hidInterface = new ws.winx.platform.windows.WinHIDInterface(__drivers);
                         #endif
                        

					 #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
						__hidInterface=new ws.winx.platform.osx.OSXHIDInterface(__drivers);
                     #endif


					#if UNITY_WEBPLAYER && !UNITY_EDITOR
						__hidInterface=new ws.winx.platform.web.WebHIDInterface(__drivers);
                    #endif

#if UNITY_ANDROID && !UNITY_EDITOR
                          __hidInterface=new ws.winx.platform.android.AndroidHIDInterface(__drivers);
#endif

                        Debug.Log(__hidInterface.GetType()+" is Initialized");
				}

				__hidInterface.DeviceDisconnectEvent+=new EventHandler<DeviceEventArgs<int>>(onRemoveDevice);
				__hidInterface.DeviceConnectEvent+=new EventHandler<DeviceEventArgs<IDevice>>(onAddDevice);

				return __hidInterface; }
		}


	   internal static void onRemoveDevice(object sender,DeviceEventArgs<int> args){

					if (_joysticks.ContainsIndex(args.data)) 
					
						_joysticks.Remove(args.data);
					
				}

		internal static void onAddDevice(object sender,DeviceEventArgs<IDevice> args){
					//do not allow duplicates
					if (_joysticks.ContainsIndex(args.data.PID)) return;

					_joysticks [args.data.PID] = args.data;

		}

		public static InputSettings Settings{
			get{  if(__settings==null) __settings=new InputSettings(); return __settings;}
		}


        /// <summary>
        /// Returns joystick list of Type T (Idea is to get Joystick and set/use some special ablitiy like SetMotor)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetJoysticks<T>()
        {
            IDeviceCollection devices = InputManager.Devices;

            List<T> Result = new List<T>();

            foreach (IDevice device in devices)
            {
                if (device.GetType() == typeof(T))
                {
                    Result.Add((T)device);

                }
                
            }

            return Result;
        }

        /// <summary>
        /// Add driver that would support custom device (see WinMMDriver,OSXDriver...for HOW TO)
        /// </summary>
        /// <param name="driver"></param>
		public static void AddDriver(IDriver driver){
            if(__drivers==null) __drivers= new List<IDriver>();
            __drivers.Add(driver);
		}



		/// <summary>
		/// Maps state to input.
		/// </summary>
		/// <param name="stateName">State name.</param>
		/// <param name="at">At.</param>
		/// <param name="combos">Combos.</param>
		public static void MapStateToInput(String stateName,int at=-1,params KeyCode[] combos){
			
			MapStateToInput(stateName,new InputCombination(combos),at);
			
		}


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name hash.</param>
        /// <param name="at">At.</param>
        /// <param name="combos">Combos.</param>
        public static void MapStateToInput(int stateNameHash, int at = -1, params KeyCode[] combos)
        {

            MapStateToInput(stateNameHash, new InputCombination(combos), at);

        }


		/// <summary>
		/// Maps state to input.
		/// </summary>
		/// <param name="stateName">State name.</param>
		/// <param name="at">At.</param>
		/// <param name="combos">Combos.</param>
		public static void MapStateToInput(String stateName,int at=-1,params int[] combos){

            MapStateToInput(stateName, new InputCombination(combos), at);
			
		}


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name Hash.</param>
        /// <param name="at">At.</param>
        /// <param name="combos">Combos.</param>
        public static void MapStateToInput(int stateNameHash, int at = -1, params int[] combos)
        {

            MapStateToInput(stateNameHash, new InputCombination(combos), at);

        }


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name Hash.</param>
        /// <param name="at">At.</param>
        /// <param name="combos">Actions. KeyExtension.Backspace.DOUBLE,...</param>
        public static void MapStateToInput(int stateNameHash, int at = -1, params InputAction[] actions)
        {

            MapStateToInput(stateNameHash, new InputCombination(actions), at);

        }


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name .</param>
        /// <param name="at">At.</param>
        /// <param name="combos">Actions. ex KeyExtension.Backspace.DOUBLE,...</param>
        public static void MapStateToInput(string stateName, int at = -1, params InputAction[] actions)
        {

            MapStateToInput(stateName, new InputCombination(actions), at);

        }


		/// <summary>
		/// Maps state to input.
		/// </summary>
		/// <param name="stateName">State name </param>
		/// <param name="codeCombination">Code combination.
		/// just "I" for KeyCode.I
		/// or "I"+InputAction.DOUBLE_DESIGNATOR 
		///	 or "I"+InputAction.DOUBLE_DESIGNATOR+InputAction.SPACE_DESIGNATOR+(other code);
		///   or just "I(x2)" depending of InputAction.DOUBLE_DESIGNATOR value
		/// </param>
		/// <param name="at">At.Valid:-1(next) or 0(primary) and 1(secondary)</param>
		public static void MapStateToInput(String stateName,String codeCombination,int at=-1){

			MapStateToInput(stateName,new InputCombination(codeCombination),at);

		}

        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateNameHash">State name hash(int) </param>
        /// <param name="codeCombination">Code combination.
        /// just "I" for KeyCode.I
        /// or "I"+InputAction.DOUBLE_DESIGNATOR 
        ///	 or "I"+InputAction.DOUBLE_DESIGNATOR+InputAction.SPACE_DESIGNATOR+(other code);
        ///   or just "I(x2)" depending of InputAction.DOUBLE_DESIGNATOR value
        /// </param>
        /// <param name="at">At.Valid:-1(next) or 0(primary) and 1(secondary)</param>
        public static void MapStateToInput(int stateNameHash, String codeCombination, int at = -1)
        {

            MapStateToInput(stateNameHash, new InputCombination(codeCombination), at);

        }


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name.</param>
        /// <param name="combos">Combos (ex. (int)KeyCode.P,(int)KeyCode.Joystick2Button12,JoystickDevice.toCode(...))</param>
        public static void MapStateToInput(String stateName, params int[] combos)
        {
            MapStateToInput(stateName, new InputCombination(combos));
        }


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name hash.</param>
        /// <param name="combos">Combos (ex. (int)KeyCode.P,(int)KeyCode.Joystick2Button12,JoystickDevice.toCode(...))</param>
        public static void MapStateToInput(int stateNameHash, params int[] combos)
        {
            MapStateToInput(stateNameHash, new InputCombination(combos));
        }


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name hash.</param>
        /// <param name="combos">Combos (ex. KeyCodeExtension.Backspace.DOUBLE,KeyCodeExtesnion.Joystick1AxisYPositive.SINGLE)</param>
        public static void MapStateToInput(int stateNameHash, params InputAction[] actions)
        {
            MapStateToInput(stateNameHash, new InputCombination(actions));
        }



        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name</param>
        /// <param name="combos">Combos (ex. KeyCodeExtension.Backspace.DOUBLE,KeyCodeExtesnion.Joystick1AxisYPositive.SINGLE)</param>
        public static void MapStateToInput(string stateName, params InputAction[] actions)
        {
            MapStateToInput(stateName, new InputCombination(actions));
        }


        /// <summary>
        /// Maps state to input.
        /// </summary>
        /// <param name="stateName">State name.</param>
        /// <param name="combination">Combination.</param>
        /// <param name="at">At.Valid:-1(next) or 0(primary) and 1(secondary)</param>
        public static void MapStateToInput(string stateName, InputCombination combination, int at = -1)
        {

            if (at > 2) throw new Exception("Only indexes 0(Primary) and 1(Secondary) input are allowed");

            int stateNameHash=Animator.StringToHash(stateName);
            InputState state;

            if (!Settings.stateInputs.ContainsKey(stateNameHash))
            {
                //create InputState and add to Dictionary of state inputs
                state = __settings.stateInputs[stateNameHash] = new InputState(stateName, stateNameHash);
            }
            else
            {
                state = __settings.stateInputs[stateNameHash];
            }

            state.Add(combination, at);

        }


		/// <summary>
		/// Maps state to input.
		/// </summary>
		/// <param name="stateName">State name hash.</param>
		/// <param name="combination">Combination.</param>
		/// <param name="at">At.Valid:-1(next) or 0(primary) and 1(secondary)</param>
		public static void MapStateToInput(int stateNameHash,InputCombination combination,int at=-1){

			if(at>2) throw new Exception("Only indexes 0(Primary) and 1(Secondary) input are allowed");
			
			
			InputState state;
			
			if(!Settings.stateInputs.ContainsKey(stateNameHash)){
				//create InputState and add to Dictionary of state inputs
				state=__settings.stateInputs[stateNameHash]=new InputState("GenState_"+stateNameHash,stateNameHash);
			}else{
				state=__settings.stateInputs[stateNameHash];
			}

			state.Add(combination,at);
	
		}

        public static bool HasInputState(int stateNameHash)
        {
            return __settings.stateInputs.ContainsKey(stateNameHash);
        }

        public static bool HasInputState(string stateName)
        {
            return InputManager.HasInputState(Animator.StringToHash(stateName));
        }
	

		//[Not tested] idea for expansion
		public static void PlayStateOnInputUp(Animator animator,int stateNameHash,int layer=0,float normalizedTime=0f){
					if(InputManager.GetInputUp(stateNameHash)) 
						animator.Play(stateNameHash,layer,normalizedTime); 
		}

		public static void PlayStateOnInputDown(Animator animator,int stateNameHash,int layer=0,float normalizedTime=0f){
			if(InputManager.GetInputDown(stateNameHash)) 
				animator.Play(stateNameHash,layer,normalizedTime); 
		}


		public static void CrossFadeStateOnInputUp(Animator animator,int stateNameHash,float transitionDuration=0.5f,int layer=0,float normailizeTime=0f){
				if(InputManager.GetInputUp(stateNameHash)) 
				animator.CrossFade(stateNameHash,transitionDuration,layer,normailizeTime);

		}

		public static void CrossFadeStateOnInputDown(Animator animator,int stateNameHash,float transitionDuration=0.5f,int layer=0,float normailizeTime=0f){
			if(InputManager.GetInputDown(stateNameHash)) 
				animator.CrossFade(stateNameHash,transitionDuration,layer,normailizeTime);
    	}



#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
        /// <summary>
		/// Loads the Input settings from InputSettings.xml and deserialize into OO structure.
		/// Create your .xml settings with InputMapper Editor
		/// </summary>
		public static InputSettings loadSettings(String path="InputSettings.xml"){
			XmlReaderSettings xmlSettings=new XmlReaderSettings();
			xmlSettings.CloseInput=true;
			xmlSettings.IgnoreWhitespace=true;


			
			//DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<int,InputCombination[]>),"Inputs","");
			DataContractSerializer serializer = new DataContractSerializer(typeof(InputSettings),"Inputs","");

			
			using(XmlReader reader=XmlReader.Create(path,xmlSettings))
			{

				__settings=(InputSettings)serializer.ReadObject(reader);

			}
			


			return __settings;
		}
#endif



   


//        #if (UNITY_WEBPLAYER || UNITY_EDITOR) && !UNITY_STANDALONE
//        /// <summary>
//        /// Loads the Input settings from InputSettings.xml and deserialize into OO structure.
//        /// Create your .xml settings with InputMapper Editor
//        /// </summary>
//        public static IEnumerator loadSettings(String path)
//        {
//             XmlReaderSettings xmlSettings=new XmlReaderSettings();
//            xmlSettings.CloseInput=true;
//            xmlSettings.IgnoreWhitespace=true;

//            if (Application.isEditor)
//                path = "file:///" + path;

//            WWW www = new WWW(path);
//          // UnityEngine.Debug.Log(path);
//            while (!www.isDone)
//            {
//                yield return null;
//            }

            
          

//            if (www.error != null)
//            {
//                UnityEngine.Debug.LogError(www.error);
//                yield break;
//            }

           
//            StringReader stringReader = new StringReader(www.text);

//            stringReader.Read();//skip BOM

//            using (XmlReader reader = XmlReader.Create(stringReader, xmlSettings))
//            {
//                __settings = new InputSettings();

//                int key;

//                InputAction action;
//                List<InputAction> actions = null;
//                InputCombination[] combinations = null;
//                string name;
//                InputState state;
//                int i;
//                //XmlNameTable nameTable = reader.NameTable;
//                //XmlNamespaceManager nsManager = new XmlNamespaceManager(nameTable);
//                //nsManager.AddNamespace("d1p1", "http://schemas.datacontract.org/2004/07/ws.winx.input");

//                reader.ReadToFollowing("d1p1:doubleDesignator");
//                __settings.doubleDesignator = reader.ReadElementContentAsString();


//                __settings.longDesignator = reader.ReadElementContentAsString();


//                __settings.spaceDesignator = reader.ReadElementContentAsString();




//                __settings.singleClickSensitivity = reader.ReadElementContentAsFloat();


//                __settings.doubleClickSensitivity = reader.ReadElementContentAsFloat();


//                __settings.longClickSensitivity = reader.ReadElementContentAsFloat();


//                __settings.combinationsClickSensitivity = reader.ReadElementContentAsFloat();

//                if (reader.ReadToFollowing("d2p1:KeyValueOfintInputState"))
//                {


//                    do
//                    {
//                        reader.ReadToDescendant("d2p1:Key");

//                        key = reader.ReadElementContentAsInt();




//                        if (reader.ReadToFollowing("d1p1:InputCombination"))
//                        {

//                            combinations = new InputCombination[2];
//                            i = 0;

//                            do
//                            {
//                                if (reader.GetAttribute("i:nil") == null)
//                                {


//                                    if (reader.ReadToDescendant("d1p1:InputAction"))
//                                    {
//                                        actions = new List<InputAction>();

//                                        do
//                                        {
//                                            reader.ReadToDescendant("d1p1:Code");

//                                            action = new InputAction(reader.ReadElementContentAsString());

//                                            actions.Add(action);

//                                        } while (reader.ReadToNextSibling("d1p1:InputAction"));


//                                    }




//                                    combinations[i++] = new InputCombination(actions);

//                                    reader.Read();//read </InputCombination>

//                                }



//                            } while (reader.ReadToNextSibling("d1p1:InputCombination"));



//                        }

//                        reader.ReadToFollowing("d1p1:Name");
//                        name = reader.ReadElementContentAsString();
//                        state = new InputState(name, key);
//                        state.combinations = combinations;
//                        __settings.stateInputs[key] = state;


//                        reader.Read();//</d2p1:KeyValueOfintInputState>

//                    } while (reader.ReadToNextSibling("d2p1:KeyValueOfintInputState"));
//                }
//            }

//            stringReader.Close();

         

          

//          //yield break;
          
//        }
//#endif

        public static void loadSettingsFromText(string text,bool readBOM=true)
        {
            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.CloseInput = true;
            xmlSettings.IgnoreWhitespace = true;
            StringReader stringReader = new StringReader(text);

          
            if(readBOM)
            stringReader.Read();//skip BOM

            using (XmlReader reader = XmlReader.Create(stringReader, xmlSettings))
            {
                __settings = new InputSettings();

                int key;

                InputAction action;
                List<InputAction> actions = null;
                InputCombination[] combinations = null;
                string name;
                InputState state;
                int i;
                //XmlNameTable nameTable = reader.NameTable;
                //XmlNamespaceManager nsManager = new XmlNamespaceManager(nameTable);
                //nsManager.AddNamespace("d1p1", "http://schemas.datacontract.org/2004/07/ws.winx.input");

                reader.ReadToFollowing("d1p1:doubleDesignator");
                __settings.doubleDesignator = reader.ReadElementContentAsString();


                __settings.longDesignator = reader.ReadElementContentAsString();


                __settings.spaceDesignator = reader.ReadElementContentAsString();




                __settings.singleClickSensitivity = reader.ReadElementContentAsFloat();


                __settings.doubleClickSensitivity = reader.ReadElementContentAsFloat();


                __settings.longClickSensitivity = reader.ReadElementContentAsFloat();


                __settings.combinationsClickSensitivity = reader.ReadElementContentAsFloat();

                if (reader.ReadToFollowing("d2p1:KeyValueOfintInputState"))
                {


                    do
                    {
                        reader.ReadToDescendant("d2p1:Key");

                        key = reader.ReadElementContentAsInt();




                        if (reader.ReadToFollowing("d1p1:InputCombination"))
                        {

                            combinations = new InputCombination[2];
                            i = 0;

                            do
                            {
                                if (reader.GetAttribute("i:nil") == null)
                                {


                                    if (reader.ReadToDescendant("d1p1:InputAction"))
                                    {
                                        actions = new List<InputAction>();

                                        do
                                        {
                                            reader.ReadToDescendant("d1p1:Code");

                                            action = new InputAction(reader.ReadElementContentAsString());

                                            actions.Add(action);

                                        } while (reader.ReadToNextSibling("d1p1:InputAction"));


                                    }




                                    combinations[i++] = new InputCombination(actions);

                                    reader.Read();//read </InputCombination>

                                }



                            } while (reader.ReadToNextSibling("d1p1:InputCombination"));



                        }

                        reader.ReadToFollowing("d1p1:Name");
                        name = reader.ReadElementContentAsString();
                        state = new InputState(name, key);
                        state.combinations = combinations;
                        __settings.stateInputs[key] = state;


                        reader.Read();//</d2p1:KeyValueOfintInputState>

                    } while (reader.ReadToNextSibling("d2p1:KeyValueOfintInputState"));
                }
            }

            stringReader.Close();

           // UnityEngine.Debug.Log("end reader");

          


        }



       

		#if UNITY_WEBPLAYER && !UNITY_EDITOR
		public static IEnumerator saveSettings(String url){

        //V1
			//WWWForm wwwForm=new WWWForm();
			//wwwForm.AddField("data",formatOutput);

			//WWW www=new WWW(url,wwwForm);

           
            string str=formatOutput();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            WWW www=new WWW(url,bytes);
           
           

			yield return www;

            if(www.error!=null) UnityEngine.Debug.LogError(www.error);
		}
#endif


        public static string formatOutput()
        {

            string HEADFORMAT = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
   "<Inputs xmlns:d1p1=\"http://schemas.datacontract.org/2004/07/ws.winx.input\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" +
    " <d1p1:doubleDesignator>{0}</d1p1:doubleDesignator>" +
    " <d1p1:longDesignator>{1}</d1p1:longDesignator>" +
    " <d1p1:spaceDesignator>{2}</d1p1:spaceDesignator>" +
    " <d1p1:singleClickSensitivity>{3}</d1p1:singleClickSensitivity>" +
    " <d1p1:doubleClickSensitivity>{4}</d1p1:doubleClickSensitivity>" +
   "  <d1p1:longClickSensitivity>{5}</d1p1:longClickSensitivity>" +
   "  <d1p1:combinationsClickSensitivity>{6}</d1p1:combinationsClickSensitivity>" +
   "  <d1p1:StateInputs xmlns:d2p1=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">" +
    " {7}" +
    "      </d1p1:StateInputs>" +
   "</Inputs>";

            string STATEFORMAT = " <d2p1:KeyValueOfintInputState>" +
      " <d2p1:Key>{0}</d2p1:Key>" +
      " <d2p1:Value>" +
       "  <d1p1:Hash>{0}</d1p1:Hash>" +
        " <d1p1:InputCombinations>" +
           " {1}" +

        " </d1p1:InputCombinations>" +
       "  <d1p1:Name>{2}</d1p1:Name>" +
      " </d2p1:Value>" +
     "</d2p1:KeyValueOfintInputState>";


            string COMBINATIONFORMAT =
                    "   <d1p1:InputCombination>" +
                   "     <d1p1:InputActions>" +
                  "{0}" +
                   "     </d1p1:InputActions>" +
                  "   </d1p1:InputCombination>";

            string ACTIONFORMAT =
                       "       <d1p1:InputAction>" +
                 "         <d1p1:Code>{0}</d1p1:Code>" +
                   "       </d1p1:InputAction>";
            string actionsString;

            // 
            Dictionary<int, InputState> stateInputs = InputManager.Settings.stateInputs;
            InputCombination[] combinations;
            InputCombination combination;

            int key;
            StringBuilder sb = new StringBuilder(10000);
            StringBuilder combinationSB = new StringBuilder(100);


            foreach (KeyValuePair<int, InputState> stateInput in stateInputs)
            {
                key = stateInput.Key;
                combinations = stateInput.Value.combinations;

                combinationSB.Length = 0;


                if ((combination = combinations[0]) != null)
                {
                    actionsString = "";

                    foreach (InputAction action in combination.actions)
                    {
                        actionsString += String.Format(ACTIONFORMAT, action.ToString());
                    }

                    combinationSB.AppendFormat(COMBINATIONFORMAT, actionsString);
                }

                if ((combination = combinations[1]) != null)
                {
                    actionsString = "";
                    foreach (InputAction action in combination.actions)
                    {
                        actionsString += String.Format(ACTIONFORMAT, action.ToString());
                    }

                    combinationSB.AppendFormat(COMBINATIONFORMAT, actionsString);
                }


                sb.AppendFormat(STATEFORMAT, key, combinationSB.ToString(), stateInput.Value.name);

                //stateInput.Value.name
            }

            InputSettings settings = InputManager.Settings;
            return String.Format(HEADFORMAT, settings.doubleDesignator, settings.longDesignator, settings.spaceDesignator, settings.singleClickSensitivity, settings.doubleClickSensitivity, settings.longClickSensitivity, settings.combinationsClickSensitivity,
                                sb.ToString());
        }

        #if UNITY_WEBPLAYER && UNITY_EDITOR
       public static void saveSettings(string path){
           XmlWriterSettings xmlSettings = new XmlWriterSettings();
           xmlSettings.Indent = true;
           xmlSettings.CloseOutput = true;//this would close stream after write 
          




           using (XmlWriter writer = XmlWriter.Create(path, xmlSettings))
           {
             
               writer.WriteRaw( formatOutput());
               



               //Write the XML to file and close the writer.
               writer.Flush();
               writer.Close();


           }


			Debug.Log(InputManager.Log());
       }
#endif

		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID)&& !UNITY_WEBPLAYER
		/// <summary>
		/// Saves the settings to InputSettings.xml.
		/// </summary>
		public static void saveSettings(String path){

			//DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<int,InputCombination[]>),"Inputs","");
			 
			DataContractSerializer serializer = new DataContractSerializer(typeof(InputSettings),"Inputs","");
			

			XmlWriterSettings xmlSettings=new XmlWriterSettings();
			xmlSettings.Indent=true;
			xmlSettings.CloseOutput=true;//this would close stream after write 
		//	xmlSettings.IndentChars="\t";
		//	xmlSettings.NewLineOnAttributes = false;
		//	xmlSettings.OmitXmlDeclaration = true;





			using(XmlWriter writer=XmlWriter.Create(path,xmlSettings))
			{
				//serializer.WriteObject(writer, __settings.stateInputs);
				serializer.WriteObject(writer,__settings);

				//Write the XML to file and close the writer.
				writer.Flush();
				writer.Close(); 


			}



		}

		#endif     
      



      

        //public void resetMap(){
        //}

		/// <summary>
		/// Gets the input of real or virutal axis(2keys used as axis) mapped to State.
		/// </summary>
		/// <returns>The input.</returns>
		/// <param name="stateNameHash">State name hash.</param>
		/// <param name="fromRange">From range.</param>
		/// <param name="toRange">To range.</param>
		/// <param name="sensitivity">Sensitivity.</param>
		/// <param name="dreadzone">Dreadzone.</param>
		/// <param name="gravity">Gravity.</param>
		public static float GetInput(int stateNameHash,float sensitivity=0.3f,float dreadzone=0.1f,float gravity=0.3f){
           //Use is mapping states so no quering keys during gameplay
            if (InputManager.EditMode) return 0f;
            
            __inputCombinations=__settings.stateInputs[stateNameHash].combinations;


            return __inputCombinations[0].GetAxis(sensitivity, dreadzone, gravity) + (__inputCombinations.Length == 2 && __inputCombinations[1] != null ? __inputCombinations[1].GetAxis(sensitivity, dreadzone, gravity) : 0);

		}

		/// <summary>
		/// Gets the input.
		/// </summary>
		/// <returns><c>true</c>, if input happen, <c>false</c> otherwise.</returns>
		/// <param name="stateNameHash">State name hash.</param>
		/// <param name="atOnce" default="false">Affect only in combo inputs!!!(default=false)Function returns true when combination pressed in row  If set to <c>true</c> function return true when all keys/buttons are pressed.</param>
		public static bool GetInput(int stateNameHash,bool atOnce=false){
            //Use is mapping states so no quering keys during gameplay
            if (InputManager.EditMode) return false;
			__inputCombinations=__settings.stateInputs[stateNameHash].combinations;
            return __inputCombinations[0].GetInput(atOnce) || (__inputCombinations.Length == 2 && __inputCombinations[1] != null && __inputCombinations[1].GetInput(atOnce));
        }

		/// <summary>
		/// Gets the input up.
		/// </summary>
		/// <returns><c>true</c>, if input binded to state happen, <c>false</c> otherwise.</returns>
		/// <param name="stateNameHash">State name hash.</param>
		public static bool GetInputUp(int stateNameHash){
            //Use is mapping states so no quering keys during gameplay
            if (InputManager.EditMode) return false;
			__inputCombinations=__settings.stateInputs[stateNameHash].combinations;
            return __inputCombinations[0].GetInputUp() || (__inputCombinations.Length == 2 && __inputCombinations[1] != null && __inputCombinations[1].GetInputUp());
		}

		/// <summary>
		/// Gets the input down.
		/// </summary>
		/// <returns><c>true</c>, if input binded to state down happen, <c>false</c> otherwise.</returns>
		/// <param name="stateNameHash">State name hash.</param>
		
		public static bool GetInputDown(int stateNameHash){
			//Use is mapping states so no quering keys during gameplay
			if (InputManager.EditMode) return false;
			__inputCombinations=__settings.stateInputs[stateNameHash].combinations;
            return __inputCombinations[0].GetInputDown() || (__inputCombinations.Length == 2 && __inputCombinations[1] != null && __inputCombinations[1].GetInputDown());
		}



		/// <summary>
		/// Log states - inputs values to console
		/// </summary>
		public static string Log(){
			StringBuilder content=new StringBuilder();
			int i;
			//primary,secondary...
			InputCombination[] combinations;

			foreach (var keyValuPair in __settings.stateInputs)
			{
				content.AppendLine("Name:"+keyValuPair.Value.name+" Key:"+keyValuPair.Key);
				combinations=keyValuPair.Value.combinations;

				for(i=0;i<combinations.Length;i++){
					if(combinations[i]!=null)
					content.AppendLine(i+": " +combinations[i].ToString());
				}

				content.AppendLine();
				 

			}


						return content.ToString();

		}


		
	
		
		
		#region JoystickDevicesCollection
		
		/// <summary>
		/// Defines a collection of JoystickAxes.
		/// </summary>
		public sealed class JoystickDevicesCollection : IDeviceCollection
		{
			#region Fields
			readonly Dictionary<int, IDevice> PIDToDevice;
				
			readonly Dictionary<byte, int> IndexToPID;
			
			
			List<IDevice> _iterationCacheList;//
			bool _isEnumeratorDirty = true;
			
			#endregion
			
			#region Constructors
			
			internal JoystickDevicesCollection()
			{
				PIDToDevice = new Dictionary<int, IDevice>();
				
				IndexToPID = new Dictionary<byte, int>();
				
			}
			
			#endregion
			
			#region Public Members
			
		
			
			
			#region IDeviceCollection implementation


			/// <summary>
			/// Remove the specified device with specified PID.
			/// </summary>
			/// <param name="PID">PI.</param>
			public void Remove(int PID)
			{
				IndexToPID.Remove((byte)PIDToDevice[PID].Index);
				PIDToDevice.Remove(PID);
				
				_isEnumeratorDirty = true;
			}
			
			/// <summary>
			/// Remove the specified device with specified index byte(0-15)
			/// </summary>
			/// <param name="index">Index.</param>
			public void Remove(byte index)
			{
				int pid = IndexToPID[index];
				IndexToPID.Remove(index);
				PIDToDevice.Remove(pid);
				
				_isEnumeratorDirty = true;
			}
			


			/// <summary>
			/// Gets the <see cref="ws.winx.input.InputManager+JoystickDevicesCollection"/> at the specified index.
			/// use case (byte)#
			/// </summary>
			/// <param name="index">Index.</param>
			public IDevice this[byte index]
			{
				get { return PIDToDevice[IndexToPID[index]]; }
			
			}
			
			
			public IDevice GetDeviceAt(int index){
				return PIDToDevice[IndexToPID[(byte)index]];
			}
			
			
			/// <summary>
			/// Gets or sets the <see cref="ws.winx.input.InputManager+JoystickDevicesCollection"/> with the specified PID.
			/// </summary>
			/// <param name="PID">PI.</param>
			public IDevice this[int PID]
			{
				get { return PIDToDevice[PID]; }
				internal set
				{
					IndexToPID[(byte)value.Index] = PID;
					PIDToDevice[PID] = value;
					
					_isEnumeratorDirty = true;
					
				}
			}
			
			
			public bool ContainsIndex(int index)
			{
				return IndexToPID.ContainsKey((byte)index);
			}
			
			public bool ContainsPID(int pid)
			{
				return PIDToDevice.ContainsKey(pid);
			}
			
			public void Clear(){
				IndexToPID.Clear();
				PIDToDevice.Clear();
			}
			
			public System.Collections.IEnumerator GetEnumerator()
			{
				if (_isEnumeratorDirty)
				{
					_iterationCacheList = PIDToDevice.Values.ToList<IDevice>();
					_isEnumeratorDirty = false;
					
					
				}
				
				return _iterationCacheList.GetEnumerator();
				
			}
			
			
			/// <summary>
			/// Gets a System.Int32 indicating the available amount of JoystickDevices.
			/// </summary>
			public int Count
			{
				get { return PIDToDevice.Count; }
			}
			
			#endregion
			
			#endregion
			
			
			
			
			
			
			
		}
		#endregion;



        public static void Dispose(){


            if (__hidInterface != null)
            {
                __hidInterface.Dispose();
                __hidInterface = null;
            }


			_joysticks.Clear();

        }



		#region Settings

		#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
		[DataContract]
#endif
		public class InputSettings{



			
			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Order=4)]
			#endif
			public float singleClickSensitivity{
				get{ return InputAction.SINGLE_CLICK_SENSITIVITY; }
				set{ InputAction.SINGLE_CLICK_SENSITIVITY=value; }

			}

			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Order=5)]
			#endif
			public float doubleClickSensitivity{
				get{ return InputAction.DOUBLE_CLICK_SENSITIVITY; }
				set{ InputAction.DOUBLE_CLICK_SENSITIVITY=value; }
				
			}

			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Order=6)]
			#endif
			public float longClickSensitivity{
				get{ return InputAction.LONG_CLICK_SENSITIVITY; }
				set{ InputAction.LONG_CLICK_SENSITIVITY=value; }
				
			}

			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Order=7)]
			#endif
			public float combinationsClickSensitivity{
				get{ return InputAction.COMBINATION_CLICK_SENSITIVITY; }
				set{ InputAction.COMBINATION_CLICK_SENSITIVITY=value; }
				
			}

			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Order=1)]
			#endif
			public string doubleDesignator{
				get{ return InputAction.DOUBLE_DESIGNATOR; }
				set{ InputAction.DOUBLE_DESIGNATOR=value; }
				
			}

			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Order=2)]
			#endif
			public string longDesignator{
				get{ return InputAction.LONG_DESIGNATOR; }
				set{ InputAction.LONG_DESIGNATOR=value; }
				
			}

			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Order=3)]
			#endif
			public string spaceDesignator{
				get{ return InputAction.SPACE_DESIGNATOR.ToString(); }
				set{ InputAction.SPACE_DESIGNATOR=value[0]; }
				
			}

			#if (UNITY_STANDALONE || UNITY_EDITOR || UNITY_ANDROID) && !UNITY_WEBPLAYER
			[DataMember(Name="StateInputs",Order=8)]
			#endif
			protected Dictionary<int,InputState> _stateInputs;
			
			public Dictionary<int,InputState> stateInputs{
				get {return _stateInputs;}
			}


		   public InputSettings(){
					_stateInputs=new Dictionary<int,InputState>();
		   }
		}
		#endregion





    }
}


