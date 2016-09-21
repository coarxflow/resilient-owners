using System;
using ColossalFramework.UI;

namespace ResilientOwners
{
	public class BookPanel : UIPanel
	{
		public override void Awake ()
		{
			atlas = StatesButton.CreateTextureAtlas("icons.book_bckgrnd.png", "ResilientOwnersBook", this.atlas.material, 512, 512, 1);

			backgroundSprite = "ResilientOwnersBook_0";
		}
	}
}

