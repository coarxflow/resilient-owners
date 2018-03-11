using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HistoricBuildings
{
	[UIComponentMenu("Multiline Text Field"), ExecuteInEditMode]
	public class UIMultilineTextField : UIInteractiveComponent
	{
		internal class UndoData
		{
			public string m_Text;
			
			public int m_Position;
		}
		
		[SerializeField]
		protected bool m_AcceptsTab;
		
		[SerializeField]
		protected bool m_IsPasswordField;
		
		[SerializeField]
		protected string m_PasswordChar = "*";
		
		[SerializeField]
		protected bool m_ReadOnly;
		
		[SerializeField]
		protected Color32 m_SelectionBackground = new Color32(0, 105, 210, 255);
		
		[SerializeField]
		protected string m_SelectionSprite;
		
		[SerializeField]
		protected RectOffset m_Padding;
		
		[SerializeField]
		protected float m_CursorBlinkTime = 0.45f;
		
		[SerializeField]
		protected int m_CursorWidth = 1;
		
		[SerializeField]
		protected int m_MaxLength = 1024;
		
		[SerializeField]
		protected bool m_SelectOnFocus;
		
		[SerializeField]
		protected bool m_NumericalOnly;
		
		[SerializeField]
		protected bool m_AllowFloats;
		
		[SerializeField]
		protected bool m_SubmitOnFocusLost = true;
		
		private int m_SelectionStart;
		
		private int m_SelectionEnd;
		
		private int m_MouseSelectionAnchor;
		
		private int m_ScrollIndex;
		
		private int m_CursorIndex;
		
		private float m_LeftOffset;
		
		private bool m_CursorShown;
		
		private float[] m_CharWidths;
		
		private float m_TimeSinceFocus;
		
		private string m_UndoText = "";
		
		private bool m_FocusForced;
		
		public static int kUndoLimit = 20;
		
		private List<UIMultilineTextField.UndoData> m_UndoData = new List<UIMultilineTextField.UndoData>(UIMultilineTextField.kUndoLimit);
		
		private int m_UndoCount;
		
		private bool m_Undoing;
		
		public event PropertyChangedEventHandler<bool> eventReadOnlyChanged;
		
		public event PropertyChangedEventHandler<string> eventPasswordCharacterChanged;
		
		public event PropertyChangedEventHandler<string> eventTextChanged;
		
		public event PropertyChangedEventHandler<string> eventTextSubmitted;
		
		public event PropertyChangedEventHandler<string> eventTextCancelled;
		
		public bool submitOnFocusLost
		{
			get
			{
				return this.m_SubmitOnFocusLost;
			}
			set
			{
				this.m_SubmitOnFocusLost = value;
			}
		}
		
		public int selectionStart
		{
			get
			{
				return this.m_SelectionStart;
			}
			set
			{
				if (value != this.m_SelectionStart)
				{
					this.m_SelectionStart = Mathf.Max(0, Mathf.Min(value, this.m_Text.Length));
					this.m_SelectionEnd = Mathf.Max(this.m_SelectionEnd, this.m_SelectionStart);
					this.Invalidate();
				}
			}
		}
		
		public int selectionEnd
		{
			get
			{
				return this.m_SelectionEnd;
			}
			set
			{
				if (value != this.m_SelectionEnd)
				{
					this.m_SelectionEnd = Mathf.Max(0, Mathf.Min(value, this.m_Text.Length));
					this.m_SelectionStart = Mathf.Max(this.m_SelectionStart, this.m_SelectionEnd);
					this.Invalidate();
				}
			}
		}
		
		public int selectionLength
		{
			get
			{
				return this.m_SelectionEnd - this.m_SelectionStart;
			}
		}
		
		public string selectedText
		{
			get
			{
				if (this.m_SelectionEnd == this.m_SelectionStart)
				{
					return "";
				}
				return this.m_Text.Substring(this.m_SelectionStart, this.selectionLength);
			}
		}
		
		public bool selectOnFocus
		{
			get
			{
				return this.m_SelectOnFocus;
			}
			set
			{
				this.m_SelectOnFocus = value;
			}
		}
		
		public bool numericalOnly
		{
			get
			{
				return this.m_NumericalOnly;
			}
			set
			{
				this.m_NumericalOnly = value;
			}
		}
		
		public bool allowFloats
		{
			get
			{
				return this.m_AllowFloats;
			}
			set
			{
				this.m_AllowFloats = value;
			}
		}
		
		public RectOffset padding
		{
			get
			{
				if (this.m_Padding == null)
				{
					this.m_Padding = new RectOffset();
				}
				return this.m_Padding;
			}
			set
			{
				value = value.ConstrainPadding();
				if (!object.Equals(value, this.m_Padding))
				{
					this.m_Padding = value;
					this.Invalidate();
				}
			}
		}
		
		public bool isPasswordField
		{
			get
			{
				return this.m_IsPasswordField;
			}
			set
			{
				if (value != this.m_IsPasswordField)
				{
					this.m_IsPasswordField = value;
					this.Invalidate();
				}
			}
		}
		
		public string passwordCharacter
		{
			get
			{
				return this.m_PasswordChar;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.m_PasswordChar = value[0].ToString();
				}
				else
				{
					this.m_PasswordChar = value;
				}
				this.OnPasswordCharacterChanged();
				this.Invalidate();
			}
		}
		
		public float cursorBlinkTime
		{
			get
			{
				return this.m_CursorBlinkTime;
			}
			set
			{
				this.m_CursorBlinkTime = value;
			}
		}
		
		public int cursorWidth
		{
			get
			{
				return this.m_CursorWidth;
			}
			set
			{
				this.m_CursorWidth = value;
			}
		}
		
		public bool readOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				if (value != this.m_ReadOnly)
				{
					this.m_ReadOnly = value;
					this.OnReadOnlyChanged();
					this.Invalidate();
				}
			}
		}
		
		public override string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (value.Length > this.maxLength)
				{
					value = value.Substring(0, this.maxLength);
				}
				value = value.Replace("\t", " ");
				if (value != this.m_Text)
				{
					this.m_Text = value;
					this.m_ScrollIndex = (this.m_CursorIndex = 0);
					this.OnTextChanged();
					this.Invalidate();
				}
			}
		}
		
		public int maxLength
		{
			get
			{
				return this.m_MaxLength;
			}
			set
			{
				if (value != this.m_MaxLength)
				{
					this.m_MaxLength = Mathf.Max(0, value);
					if (this.maxLength < this.m_Text.Length)
					{
						this.text = this.m_Text.Substring(0, this.maxLength);
					}
					this.Invalidate();
				}
			}
		}
		
		public string selectionSprite
		{
			get
			{
				return this.m_SelectionSprite;
			}
			set
			{
				if (value != this.m_SelectionSprite)
				{
					this.m_SelectionSprite = value;
					this.Invalidate();
				}
			}
		}
		
		public Color32 selectionBackgroundColor
		{
			get
			{
				return this.m_SelectionBackground;
			}
			set
			{
				this.m_SelectionBackground = value;
				this.Invalidate();
			}
		}
		
		protected internal virtual void OnTextChanged()
		{
			if (!this.m_Undoing)
			{
				this.m_UndoData.RemoveRange(this.m_UndoData.Count - this.m_UndoCount, this.m_UndoCount);
				this.m_UndoData.Add(new UIMultilineTextField.UndoData
				                    {
					m_Text = this.text,
					m_Position = this.m_CursorIndex
				});
				this.m_UndoCount = 0;
				if (UITextField.kUndoLimit != 0 && UITextField.kUndoLimit <= this.m_UndoData.Count)
				{
					this.m_UndoData.RemoveAt(0);
				}
			}
			if (this.eventTextChanged != null)
			{
				this.eventTextChanged(this, this.text);
			}
			base.Invoke("OnTextChanged", new object[]
			            {
				this.text
			});
		}
		
		protected internal virtual void OnPasswordCharacterChanged()
		{
			if (this.eventPasswordCharacterChanged != null)
			{
				this.eventPasswordCharacterChanged(this, this.passwordCharacter);
			}
			base.Invoke("OnPasswordCharacterChanged", new object[]
			            {
				this.passwordCharacter
			});
		}
		
		protected internal virtual void OnReadOnlyChanged()
		{
			if (this.eventReadOnlyChanged != null)
			{
				this.eventReadOnlyChanged(this, this.readOnly);
			}
			base.Invoke("OnReadOnlyChanged", new object[]
			            {
				this.readOnly
			});
		}
		
		protected internal virtual void OnSubmit()
		{
			this.m_FocusForced = true;
			base.Unfocus();
			if (this.eventTextSubmitted != null)
			{
				this.eventTextSubmitted(this, this.text);
			}
			base.InvokeUpward("OnTextSubmitted", new object[]
			                  {
				this.text
			});
		}
		
		protected internal virtual void OnCancel()
		{
			this.m_FocusForced = true;
			this.m_Text = this.m_UndoText;
			base.Unfocus();
			if (this.eventTextCancelled != null)
			{
				this.eventTextCancelled(this, this.text);
			}
			base.InvokeUpward("OnTextCancelled", new object[]
			                  {
				this,
				this.text
			});
		}
		
		private bool IsDigit(char character)
		{
			string numberDecimalSeparator = LocaleManager.cultureInfo.NumberFormat.NumberDecimalSeparator;
			string negativeSign = LocaleManager.cultureInfo.NumberFormat.NegativeSign;
			return char.IsDigit(character) || (this.allowFloats && character.ToString() == numberDecimalSeparator && !this.m_Text.Contains(numberDecimalSeparator)) || (this.allowFloats && character.ToString() == negativeSign && !this.m_Text.Contains(negativeSign));
		}
		
		protected override void OnKeyPress(UIKeyEventParameter p)
		{
			if (!base.builtinKeyNavigation)
			{
				base.OnKeyPress(p);
				return;
			}
			if (this.readOnly || char.IsControl(p.character))
			{
				base.OnKeyPress(p);
				return;
			}
			if (this.numericalOnly && !this.IsDigit(p.character))
			{
				base.OnKeyPress(p);
				return;
			}
			base.OnKeyPress(p);
			if (p.used)
			{
				return;
			}
			this.DeleteSelection();
			if (this.m_Text.Length < this.maxLength)
			{
				if (this.m_CursorIndex == this.m_Text.Length)
				{
					this.m_Text += p.character;
				}
				else
				{
					this.m_Text = this.m_Text.Insert(this.m_CursorIndex, p.character.ToString());
				}
				this.m_CursorIndex++;
				this.OnTextChanged();
				this.Invalidate();
			}
			p.Use();
		}
		
		public void Cancel()
		{
			this.ClearSelection();
			this.m_CursorIndex = (this.m_ScrollIndex = 0);
			this.Invalidate();
			this.OnCancel();
		}
		
		protected override void OnKeyDown(UIKeyEventParameter p)
		{
			if (!base.builtinKeyNavigation)
			{
				base.OnKeyDown(p);
				return;
			}
			if (this.readOnly)
			{
				return;
			}
			base.OnKeyDown(p);
			if (p.used)
			{
				return;
			}
			KeyCode keycode = p.keycode;
			if (keycode <= KeyCode.Escape)
			{
				if (keycode != KeyCode.Backspace)
				{
					if (keycode == KeyCode.Return)
					{
						this.OnSubmit();
						goto IL_3F1;
					}
					if (keycode == KeyCode.Escape)
					{
						this.ClearSelection();
						this.m_CursorIndex = (this.m_ScrollIndex = 0);
						this.Invalidate();
						this.OnCancel();
						goto IL_3F1;
					}
				}
				else
				{
					if (p.control)
					{
						this.DeletePreviousWord();
						goto IL_3F1;
					}
					this.DeletePreviousChar();
					goto IL_3F1;
				}
			}
			else if (keycode <= KeyCode.Z)
			{
				switch (keycode)
				{
				case KeyCode.A:
					if (p.control)
					{
						this.SelectAll();
						goto IL_3F1;
					}
					goto IL_3F1;
				case KeyCode.B:
					break;
				case KeyCode.C:
					if (p.control)
					{
						this.CopySelectionToClipboard();
						goto IL_3F1;
					}
					goto IL_3F1;
				default:
					switch (keycode)
					{
					case KeyCode.V:
					{
						if (!p.control)
						{
							goto IL_3F1;
						}
						string text = Clipboard.text;
						if (!string.IsNullOrEmpty(text))
						{
							this.PasteAtCursor(text);
							goto IL_3F1;
						}
						goto IL_3F1;
					}
					case KeyCode.X:
						if (p.control)
						{
							this.CutSelectionToClipboard();
							goto IL_3F1;
						}
						goto IL_3F1;
					case KeyCode.Y:
						if (p.control)
						{
							this.m_Undoing = true;
							try
							{
								this.m_UndoCount--;
								this.ClearSelection();
								this.text = this.m_UndoData[this.m_UndoData.Count - this.m_UndoCount - 1].m_Text;
								this.m_CursorIndex = this.m_UndoData[this.m_UndoData.Count - this.m_UndoCount - 1].m_Position;
							}
							catch
							{
								this.m_UndoCount++;
							}
							this.m_Undoing = false;
							goto IL_3F1;
						}
						goto IL_3F1;
					case KeyCode.Z:
						if (p.control)
						{
							this.m_Undoing = true;
							try
							{
								this.m_UndoCount++;
								this.ClearSelection();
								this.text = this.m_UndoData[this.m_UndoData.Count - this.m_UndoCount - 1].m_Text;
								this.m_CursorIndex = this.m_UndoData[this.m_UndoData.Count - this.m_UndoCount - 1].m_Position;
							}
							catch
							{
								this.m_UndoCount--;
							}
							this.m_Undoing = false;
							goto IL_3F1;
						}
						goto IL_3F1;
					}
					break;
				}
			}
			else if (keycode != KeyCode.Delete)
			{
				switch (keycode)
				{
				case KeyCode.RightArrow:
					if (p.control)
					{
						if (p.shift)
						{
							this.MoveSelectionPointRightWord();
							goto IL_3F1;
						}
						this.MoveToNextWord();
						goto IL_3F1;
					}
					else
					{
						if (p.shift)
						{
							this.MoveSelectionPointRight();
							goto IL_3F1;
						}
						if (this.selectionLength > 0)
						{
							this.MoveToSelectionEnd();
							goto IL_3F1;
						}
						this.MoveToNextChar();
						goto IL_3F1;
					}
					break;
				case KeyCode.LeftArrow:
					if (p.control)
					{
						if (p.shift)
						{
							this.MoveSelectionPointLeftWord();
							goto IL_3F1;
						}
						this.MoveToPreviousWord();
						goto IL_3F1;
					}
					else
					{
						if (p.shift)
						{
							this.MoveSelectionPointLeft();
							goto IL_3F1;
						}
						if (this.selectionLength > 0)
						{
							this.MoveToSelectionStart();
							goto IL_3F1;
						}
						this.MoveToPreviousChar();
						goto IL_3F1;
					}
					break;
				case KeyCode.UpArrow:
					if (p.control)
					{
						if (p.shift)
						{
							this.MoveSelectionPointLeftWord();
							goto IL_3F1;
						}
						this.MoveToPreviousWord();
						goto IL_3F1;
					}
					else
					{
						if (p.shift)
						{
							this.MoveSelectionPointUp();
							goto IL_3F1;
						}
						if (this.selectionLength > 0)
						{
							this.MoveToSelectionStart();
							//goto IL_3F1;
						}
						this.MoveToUpperChar();
						goto IL_3F1;
					}			
					break;
				case KeyCode.DownArrow:
					if (p.control)
					{
						if (p.shift)
						{
							this.MoveSelectionPointRightWord();
							goto IL_3F1;
						}
						this.MoveToNextWord();
						goto IL_3F1;
					}
					else
					{
						if (p.shift)
						{
							this.MoveSelectionPointDown();
							goto IL_3F1;
						}
						if (this.selectionLength > 0)
						{
							this.MoveToSelectionEnd();
							//goto IL_3F1;
						}
						this.MoveToLowerChar();
						goto IL_3F1;
					}
					break;
				case KeyCode.Insert:
				{
					if (!p.shift)
					{
						goto IL_3F1;
					}
					string text2 = Clipboard.text;
					if (!string.IsNullOrEmpty(text2))
					{
						this.PasteAtCursor(text2);
						goto IL_3F1;
					}
					goto IL_3F1;
				}
				case KeyCode.Home:
					if (p.shift)
					{
						this.SelectToStart();
						goto IL_3F1;
					}
					this.MoveToStart();
					goto IL_3F1;
				case KeyCode.End:
					if (p.shift)
					{
						this.SelectToEnd();
						goto IL_3F1;
					}
					this.MoveToEnd();
					goto IL_3F1;
				}
			}
			else
			{
				if (this.m_SelectionStart != this.m_SelectionEnd)
				{
					this.DeleteSelection();
					goto IL_3F1;
				}
				if (p.control)
				{
					this.DeleteNextWord();
					goto IL_3F1;
				}
				this.DeleteNextChar();
				goto IL_3F1;
			}
			base.OnKeyDown(p);
			return;
		IL_3F1:
				p.Use();
		}

		UITextField m_dummyTextField;
		bool m_retainFocus = false;

		protected override void OnGotFocus(UIFocusEventParameter p)
		{
//			base.OnGotFocus(p);
//			this.m_UndoText = this.text;
//			if (!this.readOnly)
//			{
//				this.m_TimeSinceFocus = Time.realtimeSinceStartup;
//				base.StartCoroutine(this.MakeCursorBlink());
//				if (this.selectOnFocus)
//				{
//					this.m_SelectionStart = 0;
//					this.m_SelectionEnd = this.m_Text.Length;
//				}
//			}

			//make UIview believe it has focused on a UITextView
			UIView.SetFocus(m_dummyTextField);
			//this.Invalidate();
		}

		protected override void OnEnterFocus(UIFocusEventParameter p)
		{
			base.OnEnterFocus(p);
			base.OnGotFocus(p);

			this.m_UndoText = this.text;
			if (!this.readOnly)
			{
				this.m_TimeSinceFocus = Time.realtimeSinceStartup;
				m_retainFocus = true;
				base.StartCoroutine(this.MakeCursorBlink());
				if (this.selectOnFocus)
				{
					this.m_SelectionStart = 0;
					this.m_SelectionEnd = this.m_Text.Length;
				}
			}
			this.Invalidate();
		}
		
		protected override void OnLostFocus(UIFocusEventParameter p)
		{
			//base.OnLostFocus(p);
//			if (!this.m_FocusForced)
//			{
//				if (this.submitOnFocusLost)
//				{
//					this.OnSubmit();
//				}
//				else
//				{
//					this.OnCancel();
//				}
//			}
//			this.m_FocusForced = false;
//			this.m_CursorShown = false;
//			this.ClearSelection();
//			this.Invalidate();
//			this.m_TimeSinceFocus = 0f;
		}

		//!!!! is not called for whatever reason, submit on focus lost implemented in MakeCursorBlink instead
		protected void OnLeaveFocus(UIFocusEventParameter p)
		{
			base.OnLostFocus(p);
			base.OnLeaveFocus(p);

			if (!this.m_FocusForced)
			{
				if (this.submitOnFocusLost)
				{
					this.OnSubmit();
				}
				else
				{
					this.OnCancel();
				}
			}
			this.m_retainFocus = false;
			this.m_FocusForced = false;
			this.m_CursorShown = false;
			this.ClearSelection();
			this.Invalidate();
			this.m_TimeSinceFocus = 0f;
		}
		
		protected override void OnDoubleClick(UIMouseEventParameter p)
		{
			if (!this.readOnly && p.buttons.IsFlagSet(UIMouseButton.Left))
			{
				int charIndexAt = this.GetCharIndexAt(p);
				this.SelectWordAtIndex(charIndexAt);
			}
			base.OnDoubleClick(p);
		}
		
		protected override void OnClick(UIMouseEventParameter p)
		{
			p.Use();
			base.OnClick(p);
		}
		
		protected override void OnMouseDown(UIMouseEventParameter p)
		{
			if (!this.readOnly && p.buttons.IsFlagSet(UIMouseButton.Left))
			{
				int charIndexAt = this.GetCharIndexAt(p);
				if (charIndexAt != this.m_CursorIndex)
				{
					this.m_CursorIndex = charIndexAt;
					this.m_CursorShown = true;
					this.Invalidate();
					p.Use();
				}
				this.m_MouseSelectionAnchor = this.m_CursorIndex;
				this.m_SelectionStart = (this.m_SelectionEnd = this.m_CursorIndex);
			}
			base.OnMouseDown(p);
		}
		
		protected override void OnMouseUp(UIMouseEventParameter p)
		{
			if (!this.readOnly && p.buttons.IsFlagSet(UIMouseButton.Left) && PlatformService.ShowGamepadTextInput(this.isPasswordField ? GamepadTextInputMode.TextInputModePassword : GamepadTextInputMode.TextInputModeNormal, GamepadTextInputLineMode.TextInputLineModeSingleLine, "Input", this.maxLength, this.text))
			{
				p.Use();
                PlatformService.eventSteamGamepadInputDismissed += new PlatformService.SteamGamepadInputDismissedHandler(this.OnSteamInputDismissed);
			}
			base.OnMouseUp(p);
		}
		
		private void OnSteamInputDismissed(string str)
		{
            PlatformService.eventSteamGamepadInputDismissed -= new PlatformService.SteamGamepadInputDismissedHandler(this.OnSteamInputDismissed);
			if (str != null)
			{
				this.text = str;
				this.OnSubmit();
			}
			this.MoveToEnd();
			base.Unfocus();
		}
		
		protected override void OnMouseMove(UIMouseEventParameter p)
		{
			if (!this.readOnly && this.hasFocus && p.buttons.IsFlagSet(UIMouseButton.Left))
			{
				int charIndexAt = this.GetCharIndexAt(p);
				if (charIndexAt != this.m_CursorIndex)
				{
					this.m_CursorIndex = charIndexAt;
					this.m_CursorShown = true;
					this.Invalidate();
					p.Use();
					this.m_SelectionStart = Mathf.Min(this.m_MouseSelectionAnchor, charIndexAt);
					this.m_SelectionEnd = Mathf.Max(this.m_MouseSelectionAnchor, charIndexAt);
					return;
				}
			}
			base.OnMouseMove(p);
		}


		private int m_currentLine = 0;

		private int GetCharIndexAt(UIMouseEventParameter p)
		{
			Vector2 hitPosition = base.GetHitPosition(p);
			float conv = base.PixelsToUnits();
			float meanLineHeight = stringAreaSize.y / linesCount;

			int target_char = text.Length; //last char if not any before succeeded
			float left_carriage = this.m_LeftOffset / conv+ (float)this.padding.left;
			int target_line = Mathf.Clamp (Mathf.FloorToInt (hitPosition.y / meanLineHeight), 0, linesCount - 1);;
			int current_line = 0;
			int end_of_word = 0;
			float end_of_word_carriage = 0f;
			for (int i = 0; i < this.m_CharWidths.Length; i++)
			{
				//register the index of an end of word
				if(char.IsWhiteSpace(m_Text[i]))
				{
					end_of_word = i;
					end_of_word_carriage = left_carriage;
				}
				
				left_carriage += this.m_CharWidths[i] / conv;

				//assert if target character is reached
				if ((current_line == target_line && left_carriage >= hitPosition.x))
				{
					target_char = i;
					break;
				}

				//go to next line
				if(left_carriage > stringAreaSize.x)
				{
					current_line++;
					if(current_line > target_line)
					{
						target_char = end_of_word;
						break;
					}
					i = end_of_word;//return to the first character of the word that was wrapped to the next line
					//left_carriage -= end_of_word_carriage; //get left over carrigae of the word that was wrapped to the next line
					//left_carriage += this.m_CharWidths[i] / conv;
					left_carriage = 0f;
					end_of_word_carriage = 0f;
				}

			}
			m_currentLine = current_line;
			return target_char;
		}
		
		private IEnumerator MakeCursorBlink()
		{
			if (Application.isPlaying)
			{
				this.m_CursorShown = true;
				while (this.m_retainFocus)
				{
					yield return new WaitForSeconds(this.cursorBlinkTime);
					this.m_CursorShown = !this.m_CursorShown;
					this.Invalidate();
					this.m_retainFocus = UIView.ContainsFocus(this);
				}
				//focus was lost according to UIView, save
				if (this.submitOnFocusLost)
				{
					this.OnSubmit();
				}
				else
				{
					this.OnCancel();
				}
				this.m_CursorShown = false;
				this.Invalidate();
			}
			yield break;
		}
		
		public void ClearSelection()
		{
			this.m_SelectionStart = 0;
			this.m_SelectionEnd = 0;
			this.m_MouseSelectionAnchor = 0;
		}
		
		public void MoveToStart()
		{
			this.ClearSelection();
			this.SetCursorPos(0);
		}
		
		public void MoveToEnd()
		{
			this.ClearSelection();
			this.SetCursorPos(this.m_Text.Length);
		}
		
		public void MoveToNextChar()
		{
			this.ClearSelection();
			this.SetCursorPos(this.m_CursorIndex + 1);
		}
		
		public void MoveToPreviousChar()
		{
			this.ClearSelection();
			this.SetCursorPos(this.m_CursorIndex - 1);
		}

		public int GetCharInNearbyLine(int current_char, int current_line, int offset_target_line)
		{
			//get x position of current char
			float conv = base.PixelsToUnits();
			float x_pos = this.m_LeftOffset / conv+ (float)this.padding.left;
			int end_of_word = 0;
			for (int i = 0; i < current_char; i++)
			{
				//register the index of an end of word
				if(char.IsWhiteSpace(m_Text[i]))
				{
					end_of_word = i;
				}
				
				x_pos += this.m_CharWidths[i] / conv;

				//go to next line
				if(x_pos > stringAreaSize.x)
				{
					i = end_of_word;//return to the first character of the word that was wrapped to the next line
					x_pos = 0f;
				}

			}

			//get character of upper line close to this pos
			float left_carriage = this.m_LeftOffset / conv+ (float)this.padding.left;
			int inspect_line = 0;
			int target_line = current_line+offset_target_line;
			end_of_word = 0;
			int target_char = 0;
			for (int i = 0; i < this.text.Length; i++)
			{
				//register the index of an end of word
				if(char.IsWhiteSpace(m_Text[i]))
				{
					end_of_word = i;
				}
				
				left_carriage += this.m_CharWidths[i] / conv;

				//assert if target character is reached
				if ((inspect_line == target_line && left_carriage >= x_pos))
				{
					target_char = i;
					break;
				}

				//go to next line
				if(left_carriage > stringAreaSize.x)
				{
					inspect_line++;
					if(inspect_line > target_line)
					{
						target_char = end_of_word;
						break;
					}
					i = end_of_word;//return to the first character of the word that was wrapped to the next line
					left_carriage = 0f;
				}

			}

			return target_char;
		}

		public void MoveToUpperChar()
		{
			int new_pos = 0;
			if(m_currentLine != 0)
			{
				new_pos = GetCharInNearbyLine(m_CursorIndex, m_currentLine, -1);
				m_currentLine--;
			}

			this.ClearSelection();
			this.SetCursorPos(new_pos);
		}

		public void MoveToLowerChar()
		{
			int new_pos = text.Length;
			if(m_currentLine < linesCount-1)
			{
				new_pos = GetCharInNearbyLine(m_CursorIndex, m_currentLine, +1);
				m_currentLine++;
			}

			this.ClearSelection();
			this.SetCursorPos(new_pos);
		}
		
		public void MoveToNextWord()
		{
			this.ClearSelection();
			if (this.m_CursorIndex == this.m_Text.Length)
			{
				return;
			}
			int cursorPos = this.FindNextWord(this.m_CursorIndex);
			this.SetCursorPos(cursorPos);
		}
		
		public void MoveToPreviousWord()
		{
			this.ClearSelection();
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			int cursorPos = this.FindPreviousWord(this.m_CursorIndex);
			this.SetCursorPos(cursorPos);
		}
		
		public int FindPreviousWord(int startIndex)
		{
			int i;
			for (i = startIndex; i > 0; i--)
			{
				char c = this.m_Text[i - 1];
				if (!char.IsWhiteSpace(c) && !char.IsSeparator(c) && !char.IsPunctuation(c))
				{
					break;
				}
			}
			for (int j = i; j >= 0; j--)
			{
				if (j == 0)
				{
					i = 0;
					break;
				}
				char c2 = this.m_Text[j - 1];
				if (char.IsWhiteSpace(c2) || char.IsSeparator(c2) || char.IsPunctuation(c2))
				{
					i = j;
					break;
				}
			}
			return i;
		}

		/*TODO : fix go to*/
		public int FindNextWord(int startIndex)
		{
			int length = this.m_Text.Length;
			int i = startIndex;
			for (int j = i; j < length; j++)
			{
				char c = this.m_Text[j];
				if (char.IsWhiteSpace(c) || char.IsSeparator(c) || char.IsPunctuation(c))
				{
					i = j;
				//IL_72:
						while (i < length)
					{
						char c2 = this.m_Text[i];
						if (!char.IsWhiteSpace(c2) && !char.IsSeparator(c2) && !char.IsPunctuation(c2))
						{
							break;
						}
						i++;
					}
					return i;
				}
			}
			return 0;
			//goto IL_72;
		}
		
		public void MoveSelectionPointRightWord()
		{
			if (this.m_CursorIndex == this.m_Text.Length)
			{
				return;
			}
			int num = this.FindNextWord(this.m_CursorIndex);
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionStart = this.m_CursorIndex;
				this.m_SelectionEnd = num;
			}
			else if (this.m_SelectionEnd == this.m_CursorIndex)
			{
				this.m_SelectionEnd = num;
			}
			else if (this.m_SelectionStart == this.m_CursorIndex)
			{
				this.m_SelectionStart = num;
			}
			this.SetCursorPos(num);
		}
		
		public void MoveSelectionPointLeftWord()
		{
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			int num = this.FindPreviousWord(this.m_CursorIndex);
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionEnd = this.m_CursorIndex;
				this.m_SelectionStart = num;
			}
			else if (this.m_SelectionEnd == this.m_CursorIndex)
			{
				this.m_SelectionEnd = num;
			}
			else if (this.m_SelectionStart == this.m_CursorIndex)
			{
				this.m_SelectionStart = num;
			}
			this.SetCursorPos(num);
		}
		
		public void MoveSelectionPointRight()
		{
			if (this.m_CursorIndex == this.m_Text.Length)
			{
				return;
			}
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionEnd = this.m_CursorIndex + 1;
				this.m_SelectionStart = this.m_CursorIndex;
			}
			else if (this.m_SelectionEnd == this.m_CursorIndex)
			{
				this.m_SelectionEnd++;
			}
			else if (this.m_SelectionStart == this.m_CursorIndex)
			{
				this.m_SelectionStart++;
			}
			this.SetCursorPos(this.m_CursorIndex + 1);
		}
		
		public void MoveSelectionPointLeft()
		{
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionEnd = this.m_CursorIndex;
				this.m_SelectionStart = this.m_CursorIndex - 1;
			}
			else if (this.m_SelectionEnd == this.m_CursorIndex)
			{
				this.m_SelectionEnd--;
			}
			else if (this.m_SelectionStart == this.m_CursorIndex)
			{
				this.m_SelectionStart--;
			}
			this.SetCursorPos(this.m_CursorIndex - 1);
		}

		public void MoveSelectionPointUp()
		{
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			int new_pos = 0;
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionEnd = this.m_CursorIndex;
				this.m_SelectionStart = this.m_CursorIndex - 1;
				new_pos = this.m_CursorIndex - 1;
			}
			else if (this.m_SelectionEnd == this.m_CursorIndex && m_selectionEndLine > m_selectionStartLine+1)
			{
				new_pos = GetCharInNearbyLine(this.m_SelectionEnd, m_selectionEndLine, -1);
				this.m_SelectionEnd = new_pos;
			}
			else if (this.m_SelectionStart == this.m_CursorIndex && m_selectionStartLine > 0)
			{
				new_pos = GetCharInNearbyLine(this.m_SelectionStart, m_selectionStartLine, -1);
				this.m_SelectionStart = new_pos;
			}
			this.SetCursorPos(new_pos);
		}

		public void MoveSelectionPointDown()
		{
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			int new_pos = 0;
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionEnd = this.m_CursorIndex;
				this.m_SelectionStart = this.m_CursorIndex - 1;
				new_pos = this.m_CursorIndex - 1;
			}
			else if (this.m_SelectionEnd == this.m_CursorIndex && m_selectionEndLine < linesCount-1)
			{
				new_pos = GetCharInNearbyLine(this.m_SelectionEnd, m_selectionEndLine, +1);
				this.m_SelectionEnd = new_pos;
			}
			else if (this.m_SelectionStart == this.m_CursorIndex && m_selectionStartLine < m_selectionEndLine+1)
			{
				new_pos = GetCharInNearbyLine(this.m_SelectionStart, m_selectionStartLine, +1);
				this.m_SelectionStart = new_pos;
			}
			this.SetCursorPos(new_pos);
		}
		
		public void MoveToSelectionEnd()
		{
			int selectionEnd = this.m_SelectionEnd;
			this.ClearSelection();
			this.SetCursorPos(selectionEnd);
		}
		
		public void MoveToSelectionStart()
		{
			int selectionStart = this.m_SelectionStart;
			this.ClearSelection();
			this.SetCursorPos(selectionStart);
		}
		
		public void SelectAll()
		{
			this.m_SelectionStart = 0;
			this.m_SelectionEnd = this.m_Text.Length;
			this.m_ScrollIndex = 0;
			this.SetCursorPos(0);
		}
		
		public void SelectToStart()
		{
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionEnd = this.m_CursorIndex;
			}
			else if (this.m_SelectionEnd == this.m_CursorIndex)
			{
				this.m_SelectionEnd = this.m_SelectionStart;
			}
			this.m_SelectionStart = 0;
			this.SetCursorPos(0);
		}
		
		public void SelectToEnd()
		{
			if (this.m_CursorIndex == this.m_Text.Length)
			{
				return;
			}
			if (this.m_SelectionEnd == this.m_SelectionStart)
			{
				this.m_SelectionStart = this.m_CursorIndex;
			}
			else if (this.m_SelectionStart == this.m_CursorIndex)
			{
				this.m_SelectionStart = this.m_SelectionEnd;
			}
			this.m_SelectionEnd = this.m_Text.Length;
			this.SetCursorPos(this.m_Text.Length);
		}
		
		public void SelectWordAtIndex(int index)
		{
			if (this.m_Text.Length == 0)
			{
				return;
			}
			index = Mathf.Max(Mathf.Min(this.m_Text.Length - 1, index), 0);
			char c = this.m_Text[index];
			if (!char.IsLetterOrDigit(c))
			{
				this.m_SelectionStart = index;
				this.m_SelectionEnd = index + 1;
				this.m_MouseSelectionAnchor = 0;
			}
			else
			{
				this.m_SelectionStart = index;
				int num = index;
				while (num > 0 && char.IsLetterOrDigit(this.m_Text[num - 1]))
				{
					this.m_SelectionStart--;
					num--;
				}
				this.m_SelectionEnd = index;
				int num2 = index;
				while (num2 < this.m_Text.Length && char.IsLetterOrDigit(this.m_Text[num2]))
				{
					this.m_SelectionEnd = num2 + 1;
					num2++;
				}
			}
			this.m_CursorIndex = this.m_SelectionStart;
			this.Invalidate();
		}
		
		private void CutSelectionToClipboard()
		{
			this.CopySelectionToClipboard();
			this.DeleteSelection();
		}
		
		private void CopySelectionToClipboard()
		{
			if (this.m_SelectionStart == this.m_SelectionEnd)
			{
				return;
			}
			Clipboard.text = this.m_Text.Substring(this.m_SelectionStart, this.selectionLength);
		}
		
		private void PasteAtCursor(string clipData)
		{
			this.DeleteSelection();
			StringBuilder stringBuilder = new StringBuilder(this.m_Text.Length + clipData.Length);
			stringBuilder.Append(this.m_Text);
			for (int i = 0; i < clipData.Length; i++)
			{
				char c = clipData[i];
				if (c >= ' ')
				{
					stringBuilder.Insert(this.m_CursorIndex++, c);
				}
			}
			stringBuilder.Length = Mathf.Min(stringBuilder.Length, this.maxLength);
			this.m_Text = stringBuilder.ToString();
			this.OnTextChanged();
			this.SetCursorPos(this.m_CursorIndex);
		}
		
		private void SetCursorPos(int index)
		{
			index = Mathf.Max(0, Mathf.Min(this.m_Text.Length, index));
			if (index == this.m_CursorIndex)
			{
				return;
			}
			this.m_CursorIndex = index;
			this.m_CursorShown = this.hasFocus;
			this.m_ScrollIndex = 0; //Mathf.Min(this.m_ScrollIndex, this.m_CursorIndex);
			this.Invalidate();
		}
		
		private void DeleteSelection()
		{
			if (this.m_SelectionStart == this.m_SelectionEnd)
			{
				return;
			}
			this.m_Text = this.m_Text.Remove(this.m_SelectionStart, this.selectionLength);
			this.SetCursorPos(this.m_SelectionStart);
			this.ClearSelection();
			this.OnTextChanged();
			this.Invalidate();
		}
		
		private void DeleteNextChar()
		{
			this.ClearSelection();
			if (this.m_CursorIndex >= this.m_Text.Length)
			{
				return;
			}
			this.m_Text = this.m_Text.Remove(this.m_CursorIndex, 1);
			this.m_CursorShown = true;
			this.OnTextChanged();
			this.Invalidate();
		}
		
		private void DeletePreviousChar()
		{
			if (this.m_SelectionStart != this.m_SelectionEnd)
			{
				int selectionStart = this.m_SelectionStart;
				this.DeleteSelection();
				this.SetCursorPos(selectionStart);
				return;
			}
			this.ClearSelection();
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			this.m_Text = this.m_Text.Remove(this.m_CursorIndex - 1, 1);
			this.m_CursorIndex--;
			this.m_CursorShown = true;
			this.OnTextChanged();
			this.Invalidate();
		}
		
		private void DeleteNextWord()
		{
			this.ClearSelection();
			if (this.m_CursorIndex == this.m_Text.Length)
			{
				return;
			}
			int num = this.FindNextWord(this.m_CursorIndex);
			if (num == this.m_CursorIndex)
			{
				num = this.m_Text.Length;
			}
			this.m_Text = this.m_Text.Remove(this.m_CursorIndex, num - this.m_CursorIndex);
			this.OnTextChanged();
			this.Invalidate();
		}
		
		private void DeletePreviousWord()
		{
			this.ClearSelection();
			if (this.m_CursorIndex == 0)
			{
				return;
			}
			int num = this.FindPreviousWord(this.m_CursorIndex);
			if (num == this.m_CursorIndex)
			{
				num = 0;
			}
			this.m_Text = this.m_Text.Remove(num, this.m_CursorIndex - num);
			this.OnTextChanged();
			this.SetCursorPos(num);
		}
		
		public override void OnEnable()
		{
			if (this.padding == null)
			{
				this.padding = new RectOffset();
			}
			base.OnEnable();
			if (base.size.magnitude == 0f)
			{
				base.size = new Vector2(100f, 20f);
			}
			this.m_CursorShown = false;
			this.m_CursorIndex = (this.m_ScrollIndex = 0);
			bool flag = base.font != null && base.font.isValid;
			if (Application.isPlaying && !flag)
			{
				base.font = base.GetUIView().defaultFont;
			}
		}
		
		protected override void OnRebuildRenderData()
		{
			this.CheckDefaultText();
			if (base.atlas == null || base.font == null || !base.font.isValid)
			{
				return;
			}
			if (base.textRenderData != null)
			{
				base.textRenderData.Clear();
			}
			else
			{
				UIRenderData item = UIRenderData.Obtain();
				this.m_RenderData.Add(item);
			}
			base.renderData.material = base.atlas.material;
			base.textRenderData.material = base.atlas.material;
			this.RenderBackground();
			this.RenderText();
			this.AutoHeight();
		}
		
		private string PasswordDisplayText()
		{
			return new string(this.passwordCharacter[0], this.m_Text.Length);
		}
		
		private RectOffset GetSelectionPadding()
		{
			if (base.atlas == null)
			{
				return this.padding;
			}
			UITextureAtlas.SpriteInfo backgroundSprite = this.GetBackgroundSprite();
			if (backgroundSprite == null)
			{
				return this.padding;
			}
			return backgroundSprite.border;
		}

		private Vector2 stringAreaSize;
		private int linesCount;
		private Vector2 fieldSize;

		private void RenderText()
		{
			float num = base.PixelsToUnits();
			//Vector2 vector = new Vector2(base.size.x - (float)this.padding.horizontal, base.size.y - (float)this.padding.vertical);
			Vector2 vector = new Vector2(base.size.x - (float)this.padding.horizontal, Screen.height);
			Vector3 vector2 = base.pivot.TransformToUpperLeft(base.size, base.arbitraryPivotOffset);
			Vector3 vectorOffset = new Vector3(vector2.x + (float)this.padding.left, vector2.y - (float)this.padding.top, 0f) * num;
			string text = (this.isPasswordField && !string.IsNullOrEmpty(this.passwordCharacter)) ? this.PasswordDisplayText() : this.m_Text;
			Color32 defaultColor = base.isEnabled ? base.textColor : base.disabledTextColor;
			float textScaleMultiplier = this.GetTextScaleMultiplier();
			using (UIFontRenderer uIFontRenderer = base.font.ObtainRenderer())
			{
				uIFontRenderer.wordWrap = true;
				uIFontRenderer.maxSize = vector;
				uIFontRenderer.pixelRatio = num;
				uIFontRenderer.textScale = base.textScale * textScaleMultiplier;
				uIFontRenderer.characterSpacing = base.characterSpacing;
				uIFontRenderer.vectorOffset = vectorOffset;
				uIFontRenderer.multiLine = true;
				uIFontRenderer.textAlign = UIHorizontalAlignment.Left;
				uIFontRenderer.processMarkup = base.processMarkup;
				uIFontRenderer.colorizeSprites = base.colorizeSprites;
				uIFontRenderer.defaultColor = defaultColor;
				uIFontRenderer.bottomColor = (base.useGradient ? new Color32?(base.bottomColor) : null);
				uIFontRenderer.overrideMarkupColors = false;
				uIFontRenderer.opacity = base.CalculateOpacity();
				uIFontRenderer.outline = base.useOutline;
				uIFontRenderer.outlineSize = base.outlineSize;
				uIFontRenderer.outlineColor = base.outlineColor;
				uIFontRenderer.shadow = base.useDropShadow;
				uIFontRenderer.shadowColor = base.dropShadowColor;
				uIFontRenderer.shadowOffset = base.dropShadowOffset;
				this.m_CursorIndex = Mathf.Min(this.m_CursorIndex, text.Length);
				this.m_ScrollIndex = 0;//Mathf.Min(Mathf.Min(this.m_ScrollIndex, this.m_CursorIndex), text.Length);
				this.m_CharWidths = uIFontRenderer.GetCharacterWidths(text);
				Vector2 vector3 = vector * num;
				this.m_LeftOffset = 0f;
				if (this.horizontalAlignment == UIHorizontalAlignment.Left)
				{
//					float num2 = 0f;
//					for (int i = this.m_ScrollIndex; i < this.m_CursorIndex; i++)
//					{
//						num2 += this.m_CharWidths[i];
//					}
//					while (num2 >= vector3.x)
//					{
//						if (this.m_ScrollIndex >= this.m_CursorIndex)
//						{
//							break;
//						}
//						num2 -= this.m_CharWidths[this.m_ScrollIndex++];
//					}
				}
				else
				{
					this.m_ScrollIndex = 0;//Mathf.Max(0, Mathf.Min(this.m_CursorIndex, text.Length - 1));
					float num3 = 0f;
					float num4 = (float)base.font.size * 1.25f * num;
					while (this.m_ScrollIndex > 0 && num3 < vector3.x - num4)
					{
						num3 += this.m_CharWidths[this.m_ScrollIndex--];
					}
					float num5 = (text.Length > 0) ? uIFontRenderer.GetCharacterWidths(text.Substring(this.m_ScrollIndex)).Sum() : 0f;
					switch (this.horizontalAlignment)
					{
					case UIHorizontalAlignment.Center: //will probably not work, since let offset should change at every line
						this.m_LeftOffset = Mathf.Max(0f, (vector3.x - num5) * 0.5f);
						break;
					case UIHorizontalAlignment.Right:
						this.m_LeftOffset = Mathf.Max(0f, vector3.x - num5);
						break;
					}
					vectorOffset.x += this.m_LeftOffset;
					uIFontRenderer.vectorOffset = vectorOffset;
				}

				//get additional info to estimate line heights
				stringAreaSize = uIFontRenderer.MeasureString(text);
				fieldSize = new Vector2(this.width - this.padding.left - this.padding.right, this.height - this.padding.top - this.padding.bottom);
				linesCount = 1;
				if(uIFontRenderer.GetType().Equals(typeof(UIDynamicFont.DynamicFontRenderer)))
					linesCount = (uIFontRenderer as UIDynamicFont.DynamicFontRenderer).lineCount;
				else if(uIFontRenderer.GetType().Equals(typeof(UIBitmapFont.BitmapFontRenderer)))
					linesCount = (uIFontRenderer as UIBitmapFont.BitmapFontRenderer).LineCount;

				//rendering
				if (this.m_SelectionEnd != this.m_SelectionStart)
				{
					this.RenderSelection(this.m_ScrollIndex, this.m_CharWidths, this.m_LeftOffset);
				}
				uIFontRenderer.Render(text.Substring(this.m_ScrollIndex), base.textRenderData);
				if (this.m_CursorShown && this.m_SelectionEnd == this.m_SelectionStart)
				{
					this.RenderCursor(this.m_ScrollIndex, this.m_CursorIndex, this.m_CharWidths, this.m_LeftOffset);
				}
			}
		}

		private int m_selectionStartLine = 0;
		private int m_selectionEndLine = 0;
		private void RenderSelection(int scrollIndex, float[] charWidths, float leftOffset)
		{
			if (string.IsNullOrEmpty(this.selectionSprite) || base.atlas == null)
			{
				return;
			}
			float conv = base.PixelsToUnits();


			// get x start and end of selection, plus the number of lines it spans
			float x_start = 0f;
			int start_line = 0;
			float num = 0f;
			int i = 0;
			int end_of_word = 0;
			for (; i < selectionStart; i++)
			{
				num += charWidths[i];

				//register the index of an end of word
				if(char.IsWhiteSpace(m_Text[i]))
				{
					end_of_word = i;
				}

				//go to next line
				if(num > stringAreaSize.x*conv)
				{
					start_line++;
					i = end_of_word;
					num = 0f;
				}
			}
			x_start = num;
			float x_end = 0f;
			int end_line = start_line;
			for (; i < selectionEnd; i++)
			{
				num += charWidths[i];

				//register the index of an end of word
				if(char.IsWhiteSpace(m_Text[i]))
				{
					end_of_word = i;
				}

				//go to next line
				if(num > stringAreaSize.x*conv)
				{
					end_line++;
					i = end_of_word;
					num = 0f;
				}
			}
			x_end = num;
			m_selectionStartLine = start_line;
			m_selectionEndLine = end_line;

			float meanLineHeight = stringAreaSize.y / linesCount;

			float num9 = (meanLineHeight - (float)this.padding.vertical) * conv;
			float left1 = x_start + leftOffset + (float)this.padding.left * conv;
			float top1 = (float)(-(float)this.padding.top - start_line*meanLineHeight) * conv;
			float right1 = 0f;
			if(end_line == start_line)
			{
				right1 = left1 + (x_end - x_start);
			}
			else
			{
				right1 = (this.size.x - (float)this.padding.right) * conv;
			}
			float bottom1 = top1 - num9;

			Vector3 b = base.pivot.TransformToUpperLeft(base.size, base.arbitraryPivotOffset) * conv;
			Color32 color = base.ApplyOpacity(this.selectionBackgroundColor);
			UITextureAtlas.SpriteInfo spriteInfo = base.atlas[this.selectionSprite];
			Rect region = spriteInfo.region;
			float num12 = region.width / spriteInfo.pixelSize.x;
			float num13 = region.height / spriteInfo.pixelSize.y;

			this.AddTriangles(base.renderData.triangles, base.renderData.vertices.Count);
			Vector3 item = new Vector3(left1, top1) + b;
			Vector3 item2 = new Vector3(right1, top1) + b;
			Vector3 item3 = new Vector3(left1, bottom1) + b;
			Vector3 item4 = new Vector3(right1, bottom1) + b;
			base.renderData.vertices.Add(item);
			base.renderData.vertices.Add(item2);
			base.renderData.vertices.Add(item4);
			base.renderData.vertices.Add(item3);
			base.renderData.colors.Add(color);
			base.renderData.colors.Add(color);
			base.renderData.colors.Add(color);
			base.renderData.colors.Add(color);
			base.renderData.uvs.Add(new Vector2(region.x + num12, region.yMax - num13));
			base.renderData.uvs.Add(new Vector2(region.xMax - num12, region.yMax - num13));
			base.renderData.uvs.Add(new Vector2(region.xMax - num12, region.y + num13));
			base.renderData.uvs.Add(new Vector2(region.x + num12, region.y + num13));


			//render a rect spanning the whole width for several lines
			if(end_line > start_line + 1)
			{
				float left2 = (float)this.padding.left * conv;
				float top2 = (float)(-(float)this.padding.top - (start_line+1)*meanLineHeight) * conv;
				float right2 = (this.size.x - (float)this.padding.right) * conv;
				float bottom2 = top2 - (end_line - start_line - 1) * num9;

				this.AddTriangles(base.renderData.triangles, base.renderData.vertices.Count);
				Vector3 item5 = new Vector3(left2, top2) + b;
				Vector3 item6 = new Vector3(right2, top2) + b;
				Vector3 item7 = new Vector3(left2, bottom2) + b;
				Vector3 item8 = new Vector3(right2, bottom2) + b;
				base.renderData.vertices.Add(item5);
				base.renderData.vertices.Add(item6);
				base.renderData.vertices.Add(item7);
				base.renderData.vertices.Add(item8);
				base.renderData.colors.Add(color);
				base.renderData.colors.Add(color);
				base.renderData.colors.Add(color);
				base.renderData.colors.Add(color);
				base.renderData.uvs.Add(new Vector2(region.x + num12, region.yMax - num13));
				base.renderData.uvs.Add(new Vector2(region.xMax - num12, region.yMax - num13));
				base.renderData.uvs.Add(new Vector2(region.xMax - num12, region.y + num13));
				base.renderData.uvs.Add(new Vector2(region.x + num12, region.y + num13));
			}

			//render a rect for the last line
			if(end_line > start_line)
			{
				float left3 = (float)this.padding.left * conv;
				float top3 = (float)(-(float)this.padding.top - (end_line)*meanLineHeight) * conv;
				float right3 = x_end + leftOffset + (float)this.padding.left * conv;
				float bottom3 = top3 - num9;

				this.AddTriangles(base.renderData.triangles, base.renderData.vertices.Count);
				Vector3 item5 = new Vector3(left3, top3) + b;
				Vector3 item6 = new Vector3(right3, top3) + b;
				Vector3 item7 = new Vector3(left3, bottom3) + b;
				Vector3 item8 = new Vector3(right3, bottom3) + b;
				base.renderData.vertices.Add(item5);
				base.renderData.vertices.Add(item6);
				base.renderData.vertices.Add(item7);
				base.renderData.vertices.Add(item8);
				base.renderData.colors.Add(color);
				base.renderData.colors.Add(color);
				base.renderData.colors.Add(color);
				base.renderData.colors.Add(color);
				base.renderData.uvs.Add(new Vector2(region.x + num12, region.yMax - num13));
				base.renderData.uvs.Add(new Vector2(region.xMax - num12, region.yMax - num13));
				base.renderData.uvs.Add(new Vector2(region.xMax - num12, region.y + num13));
				base.renderData.uvs.Add(new Vector2(region.x + num12, region.y + num13));
			}
		}
		
		private void RenderCursor(int startIndex, int cursorIndex, float[] charWidths, float leftOffset)
		{
			if (string.IsNullOrEmpty(this.selectionSprite) || base.atlas == null)
			{
				return;
			}
			float num = 0f;
//			for (int i = startIndex; i < cursorIndex; i++)
//			{
//				num += charWidths[i];
//			}
			//float left_carriage = leftOffset + (float)this.padding.left;
			float conv = base.PixelsToUnits();
			int current_line = 0;
			int end_of_word = 0;
			float x_pos = 0f;
			for (int i = startIndex; i < m_Text.Length; i++)
			{
				//is target reached? inspect a few chars further in case of wrapping
				if(i == cursorIndex)
				{
					x_pos = num;
				}

				//register the index of an end of word
				if(char.IsWhiteSpace(m_Text[i]))
				{
					end_of_word = i;
					if(x_pos != 0f)
						break;
				}


				num += charWidths[i];
				
				//go to next line and wrap word
				if(num > fieldSize.x*conv)
				{
					current_line++;
					i = end_of_word;
					if(x_pos != 0f) //word wrap, char position must be assessed again
						x_pos = 0f;
					num = 0f;
				}
			}
			if(cursorIndex == m_Text.Length)
				x_pos = num;

			float meanLineHeight = stringAreaSize.y / linesCount;
			float num3 = (x_pos + leftOffset + (float)this.padding.left * conv).Quantize(conv);
			float num4 = (float)(-(float)this.padding.top - current_line*meanLineHeight) * conv;
			float num5 = conv * base.GetUIView().ratio * (float)this.cursorWidth;
			float num6 = (/*this.size.y/2*/meanLineHeight - (float)this.padding.vertical) * conv;
			Vector3 a = new Vector3(num3, num4);
			Vector3 a2 = new Vector3(num3 + num5, num4);
			Vector3 a3 = new Vector3(num3 + num5, num4 - num6);
			Vector3 a4 = new Vector3(num3, num4 - num6);
			PoolList<Vector3> vertices = base.renderData.vertices;
			PoolList<int> triangles = base.renderData.triangles;
			PoolList<Vector2> uvs = base.renderData.uvs;
			PoolList<Color32> colors = base.renderData.colors;
			Vector3 b = base.pivot.TransformToUpperLeft(base.size, base.arbitraryPivotOffset) * conv;
			this.AddTriangles(triangles, vertices.Count);
			vertices.Add(a + b);
			vertices.Add(a2 + b);
			vertices.Add(a3 + b);
			vertices.Add(a4 + b);
			Color32 item = base.ApplyOpacity(base.textColor);
			colors.Add(item);
			colors.Add(item);
			colors.Add(item);
			colors.Add(item);
			UITextureAtlas.SpriteInfo spriteInfo = base.atlas[this.selectionSprite];
			Rect region = spriteInfo.region;
			uvs.Add(new Vector2(region.x, region.yMax));
			uvs.Add(new Vector2(region.xMax, region.yMax));
			uvs.Add(new Vector2(region.xMax, region.y));
			uvs.Add(new Vector2(region.x, region.y));
		}

		static readonly int[] kTriangleIndices = new int[]
		{
			0,
			1,
			3,
			3,
			1,
			2
		};

		private void AddTriangles(PoolList<int> triangles, int baseIndex)
		{
			for (int i = 0; i < kTriangleIndices.Length; i++)
			{
				triangles.Add(kTriangleIndices[i] + baseIndex);
			}
		}

		//input values to make textfield correctly editable
		public override void Awake()
		{
			base.Awake ();
			m_SelectionSprite = "EmptySprite";
			//m_SelectOnFocus = true;
			focusedBgSprite = "TextFieldPanel";
			builtinKeyNavigation = true;
			hoveredBgSprite = "TextFieldPanelHovered";
			this.canFocus = true;
//			this.m_FocusForced = true;
//			this.m_ProcessMarkup = true;
//			this.m_IsDisposing = true;
			this.horizontalAlignment = UIHorizontalAlignment.Left;
			this.readOnly = false;
			this.submitOnFocusLost = true;

			m_dummyTextField = this.AddUIComponent<UITextField> ();
			m_dummyTextField.text = "dummy";
			m_dummyTextField.AlignTo(this, UIAlignAnchor.TopLeft);
			m_dummyTextField.isVisible = false;
		}

		//use dummy text field as title
		bool m_showTitle = false;
		public bool showTitle {
			get {
				return m_showTitle;
			}
			set {
				m_showTitle = value;
				m_dummyTextField.isVisible = m_showTitle;
			}
		}
		string m_title = "title";
		public string title {
			get
			{
				return m_title;
			}
			set
			{
				m_title = value;
				m_dummyTextField.text = m_title;
			}
		}
		public UITextField titleTextField {
			get
			{
				return m_dummyTextField;
			}
		}

		private Vector3 m_InitialRelativePosition = Vector3.zero;
		public void FixPositionAndActivateAutoHeight()
		{
			m_InitialRelativePosition = relativePosition;
			m_autoAdjustHeight = true;
			this.Invalidate ();
		}

		public float getComposedHeight()
		{
			float total_height = 0f;
			if(m_showTitle)
			{
				total_height += m_dummyTextField.height;
			}
			
			total_height += this.height;

			return total_height;
		}

		bool m_autoAdjustHeight = false;
		float m_lastHeight = 0f;
		public void AutoHeight()
		{
//			if (!m_autoAdjustHeight)
//				return;

			//relativePosition = m_InitialRelativePosition;
			float total_height = 0f;
			if(m_showTitle)
			{
				total_height += m_dummyTextField.height;
				m_dummyTextField.relativePosition = new Vector3(-5f, -total_height, 0f);
				//relativePosition += new Vector3(0, (int) total_height, 0);
			}
			else
				this.m_Padding.top = 0;
			
			total_height += stringAreaSize.y;

			//this.height = total_height;
			this.height = stringAreaSize.y;

			if (m_lastHeight != total_height) {
				m_lastHeight = total_height;
				this.Invalidate();
				eventHeightChange(this, total_height);
			}
		}

		public delegate void HeightChangedEventHandler (UIComponent component, float height);

		public event HeightChangedEventHandler eventHeightChange;

		//default text handling
		public string m_defaultText;
		public string defaultText{
			get {
				return m_defaultText;
			}
			set {
				m_defaultText = value;
			}
		}

		private Color32 m_savedTextColor = Color.black;
		void CheckDefaultText()
		{
			m_savedTextColor = textColor;
			if(text.Length == 0 || text.Equals(defaultText))
			{
				text = defaultText;
				selectOnFocus = true;
				//m_savedTextColor = textColor;
				//textColor = this.disabledTextColor;
			}
			else
			{
				selectOnFocus = false;
				//if(m_savedTextColor != Color.black)
					//extColor = m_savedTextColor;
			}
		}
	}
}
