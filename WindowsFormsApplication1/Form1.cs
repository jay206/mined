using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
	public partial class Form1 : Form
	{
		Button[][] btns;// = new Button[5][];
		CMineInfo[] mines = new CMineInfo[0];
		List<CMineInfo> Mines = new List<CMineInfo>();

		public int Rows { get; set; }
		public int Columns { get; set; }

		public Form1()
		{
			InitializeComponent();
			Rows = 10;
			Columns = 10;
			
			int w = 25;	// width
			int h = 25;	// height

			// create buttons as array of arrays
			btns = new Button[Rows][];
			for (int i = 0; i < Rows; i++) {
				btns[i] = new Button[Columns];
				for (int j = 0; j < btns[i].Length; j++) {
					btns[i][j] = new Button();
					btns[i][j].Width = w;
					btns[i][j].Height = h;
					btns[i][j].Location = new Point(j * h,i * w);
					btns[i][j].Tag = new CMineInfo(i, j, 0);// { Button = btns[i][j] };

					btns[i][j].Click += new EventHandler(btn_Click);
				}
				Controls.AddRange(btns[i]);
			}
			// set references to adjacent nodes N,S,E,W,NE,NW,SE,SW
			for (int i = 0; i < Rows; i++) {
				for (int j = 0; j < Columns; j++) {
					CMineInfo mine = (CMineInfo)btns[i][j].Tag;
					if (0 < i) {
						mine.N = (CMineInfo)btns[i - 1][j].Tag;
					}
					if (i < Rows - 1) {
						mine.S = (CMineInfo)btns[i + 1][j].Tag;
					}
					if (0 < j) {
						mine.W = (CMineInfo)btns[i][j - 1].Tag;
						if (mine.N != null) {
							mine.NW = mine.N.W;
						}
						if (mine.S != null) {
							mine.SW = mine.S.W;
						}
					}
					if (j < Columns - 1) {
						mine.E = (CMineInfo)btns[i][j + 1].Tag;
						if (mine.N != null) {
							mine.NE = mine.N.E;
						}
						if (mine.S != null) {
							mine.SE = mine.S.E;
						}						
					}
				}
			}
		}

		void btn_Click(object sender, EventArgs e)
		{
			CMineInfo mine = (CMineInfo)((Button)sender).Tag;
			//Debug.Write(((CMineInfo)((Button)sender).Tag).ToString());
			Debug.WriteLine(mine.ToString());

			// performing here as to avoid user every selecting mine on first button click
			if (0>=mines.Length) { 

			//	AddMine(1, 1);
			//	AddMine(1, 4);
			//	AddMine(2, 3);
			//	AddMine(2, 0);
				AddMine(1, 0);
				AddMine(0, 1);
				AddMine(2, 2);
				AddMine(3, 3);
				AddMine(8, 8);

				// use loop create array of CMineInfo
				// use random number to select row
				// use random number to select col
				// confirm mine not already exists otherwise regenerate random indices until valid
				// add button tag mineinfo object to array use object stored in tag property, objcts are all already created
				
				// lastly, loop through mines set Z property to indicate mines present
				for (int i = 0; i < Mines.Count; i++) {
					Mines[i].Z = 1;
				}
			}

			ClickButton((Button)sender);
		}

		public void ClickButton(Button btn)
		{
			// check if button previously removed
			if (-1 < Controls.IndexOf(btn)) {
				
				CMineInfo mine = (CMineInfo)btn.Tag;

				// count bounding mines
				mine.Count = mine.GetCount();

				// create label to replace button
				Label l = new Label();
				l.Text = mine.Count.ToString();
				l.Location = btn.Location;
				l.Size = btn.Size;
				l.AutoSize = false;
				l.TextAlign = ContentAlignment.MiddleCenter;
				int x = Controls.IndexOf(btn);
				Controls.RemoveAt(x);
				Controls.Add(l);
				Controls.SetChildIndex(l, x);

				// recursively call for removing buttons
				// note, this not most efficient method ... per Neil
				if (0 == mine.Count) {
					if (mine.N != null) {
						ClickButton(btns[mine.N.Row][mine.N.Column]);
					}
					if (mine.S != null) {
						ClickButton(btns[mine.S.Row][mine.S.Column]);
					}
					if (mine.E != null) {
						ClickButton(btns[mine.E.Row][mine.E.Column]);
					}
					if (mine.W != null) {
						ClickButton(btns[mine.W.Row][mine.W.Column]);
					}
				}
			}
		}
		private void AddMine(int r, int c)
		{
			CMineInfo mine = (CMineInfo)btns[r][c].Tag;

			// only mines have Count < 0
			mine.Count = -1;

			Mines.Add(mine);
		}
		private void Form1_Load(object sender, EventArgs e)
		{

		}
		public Button GetButton(CMineInfo mine)
		{
			return btns[mine.Row][mine.Column];
		}
	}
	public class CMineInfo
	{
		public CMineInfo()
		{
			X = Y = Z = -1;
		}
		public CMineInfo(int row, int column, int count)
		{
			Row = row;
			Column = column;
			Count = count;
		}
		public int Row { get; set; }
		public int Column { get; set; }
		public int Count { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		public CMineInfo N { get; set; }
		public CMineInfo S { get; set; }
		public CMineInfo E { get; set; }
		public CMineInfo W { get; set; }
		public CMineInfo NW { get; set; }
		public CMineInfo NE { get; set; }
		public CMineInfo SW { get; set; }
		public CMineInfo SE { get; set; }
		public Button Button { get; set; }
		public int GetCount()
		{
			if (0 > Count) {
				return -1;
			}
			int y = 0;
		//	if (0 < Z)
		//		return 0;
			if (E != null)
				y += E.Z;
			if (W != null)
				y += W.Z;
			if (N != null) {
				y += N.Z;
				if (E != null)
					y += N.E.Z; //y += N.E.Count; //
				if (W != null)
					y += N.W.Z; //y += N.W.Count; //
			}
			if (S != null) {
				y += S.Z;
				if (E != null)
					y += S.E.Z; //y += S.E.Count; //
				if (W != null)
					y += S.W.Z; //y += S.W.Count; //
			}
			return y;
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
		//	sb.AppendFormat("{{X={0},Y={1},Z={2}", X, Y, Z);
			sb.AppendFormat("{{Row={0},Column={1},Count={2}", Row, Column, Count);
			if (E != null)
				sb.AppendFormat(",E={0}", E.GetHashCode());
			if (W != null)
				sb.AppendFormat(",W={0}", W.GetHashCode());
			if (N != null) {
				sb.AppendFormat(",N={0}", N.GetHashCode());
				if (NE != null)
					sb.AppendFormat(",NE={0}", NE.GetHashCode());
				if (NW != null)
					sb.AppendFormat(",NW={0}", NW.GetHashCode());
			}
			if (S!=null) {
				sb.AppendFormat(",S={0}", S.GetHashCode());
				if (SE != null)
					sb.AppendFormat(",SE={0}", SE.GetHashCode());
				if (SW != null)
					sb.AppendFormat(",SW={0}", SW.GetHashCode());
			}
			sb.Append("}");
			return sb.ToString();
		}
	}
}
