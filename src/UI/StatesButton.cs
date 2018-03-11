using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;

namespace HistoricBuildings
{
	public class StatesButton
	{
		public UIMultiStateButton msb;

		public StatesButton (UIComponent component, int spriteWidth, int spriteHeight, int spriteCountHorizontal, string icons_atlas, string name, string[] tooltips)
		{
			msb = component.AddUIComponent<UIMultiStateButton> ();

			UITextureAtlas atlas = CreateTextureAtlas(icons_atlas, name, msb.atlas.material, spriteWidth, spriteHeight, spriteCountHorizontal);
			msb.name = name;

			msb.atlas = atlas;
			for(int i = 0; i < spriteCountHorizontal; i++)
			{
				msb.backgroundSprites[i].normal = name+"_"+i;
				if(i < spriteCountHorizontal-1)
				{
					msb.backgroundSprites.AddState();
					msb.foregroundSprites.AddState();
				}
			}
			msb.tooltip = tooltips[0];
			if(tooltips.Length == spriteCountHorizontal)
			{
				msb.eventActiveStateIndexChanged += (component2, value) => {
					msb.tooltip = tooltips[value];
				};
			}
			msb.width = spriteWidth;
			msb.height = spriteHeight;

		}

		public void SetState(int n)
		{
			msb.activeStateIndex = n;
		}

		/*********** custom icons *************/

		public static UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, int spriteCountHor) {

			Texture2D tex = new Texture2D(spriteWidth * spriteCountHor, spriteHeight, TextureFormat.ARGB32, false);
			tex.filterMode = FilterMode.Bilinear;

			try
			{ // LoadTexture
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				System.IO.Stream textureStream = assembly.GetManifestResourceStream("HistoricBuildings." + textureFile);

				byte[] buf = new byte[textureStream.Length];  //declare arraysize
				textureStream.Read(buf, 0, buf.Length); // read from stream to byte array

				tex.LoadImage(buf);

				tex.Apply(true, true);
			}
			catch (Exception ex)
			{
				CODebug.Log(LogChannel.Modding, "error opening texture file: "+ex.Message);
			}

			UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();

			try
			{ // Setup atlas
				Material material = (Material)Material.Instantiate(baseMaterial);
				material.mainTexture = tex;
				atlas.material = material;
				atlas.name = atlasName;
			}
			catch
			{
				CODebug.Log(LogChannel.Modding, "error setting texture");
			}

			// Add sprites
			for (int i = 0; i < spriteCountHor; ++i) {
				float uw = 1.0f / spriteCountHor;

				var spriteInfo = new UITextureAtlas.SpriteInfo() {
					name = atlasName+"_"+i,
					texture = tex,
					region = new Rect(i * uw, 0, uw, 1),
				};

				atlas.AddSprite(spriteInfo);
			}

			return atlas;
		}
	}
}

