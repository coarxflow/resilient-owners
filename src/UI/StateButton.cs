using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;

namespace ResilientOwners
{
	public class StateButton
	{
		public UIMultiStateButton msb;

		public StateButton (UIComponent component, int spriteWidth, int spriteHeight, string[] spriteNames, string icons_atlas)
		{
			msb = component.AddUIComponent<UIMultiStateButton> ();

			UITextureAtlas atlas = CreateTextureAtlas(icons_atlas, "ResilientUI", msb.atlas.material, spriteWidth, spriteHeight, spriteNames);
			msb.name = "Resilient MultiStateButton";

			msb.atlas = atlas;
			for(int i = 0; i < spriteNames.Length; i++)
			{
				msb.backgroundSprites[i].normal = spriteNames[i];
				if(i < spriteNames.Length-1)
				{
					msb.backgroundSprites.AddState();
					msb.foregroundSprites.AddState();
				}
			}
			msb.tooltip = "Resilient Owners toggle";
			msb.width = spriteWidth;
			msb.height = spriteHeight;

		}

		public void SetState(int n)
		{
			msb.activeStateIndex = n;
		}

		/*********** custom icons *************/

		UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames) {

			Texture2D tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false);
			tex.filterMode = FilterMode.Bilinear;

			try
			{ // LoadTexture
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
				System.IO.Stream textureStream = assembly.GetManifestResourceStream("ResilientOwners." + textureFile);

				byte[] buf = new byte[textureStream.Length];  //declare arraysize
				textureStream.Read(buf, 0, buf.Length); // read from stream to byte array

				tex.LoadImage(buf);

				tex.Apply(true, true);
				tex.width /= spriteNames.Length;
			}
			catch
			{
				CODebug.Log(LogChannel.Modding, "error opening texture file");
			}

			UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();

			try
			{ // Setup atlas
				Material material = (Material)Material.Instantiate(baseMaterial);
				material.mainTexture = tex;

				material.mainTextureScale = new Vector2(1f/spriteNames.Length, 1f);

				atlas.material = material;
				atlas.name = atlasName;
			}
			catch
			{
				CODebug.Log(LogChannel.Modding, "error setting texture");
			}

			// Add sprites
			for (int i = 0; i < spriteNames.Length; ++i) {
				float uw = 1.0f / spriteNames.Length;

				var spriteInfo = new UITextureAtlas.SpriteInfo() {
					name = spriteNames[i],
					texture = tex,
					region = new Rect(i * uw, 0, uw, 1),
				};

				atlas.AddSprite(spriteInfo);
			}

			return atlas;
		}
	}
}

