﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TES30.API;

namespace TES30.Controls
{
    public partial class GameView : Control
    {
        private static GameView instance;
        public new char BackColor
        {
            get
            {
                return TES30.API.Game.BackgroundColor;
            }
        }
        private bool playing;
        public static GameView Instance
        {
            get
            {
                return instance;
            }
        }

        public bool Playing
        {
            get
            {
                return playing;
            }

            set
            {
                playing = value;
                Invalidate();
                if (value == true)
                    ticker.Start();
                else ticker.Stop();
            }
        }
        System.Timers.Timer ticker = new System.Timers.Timer(0.0166666666666667d);
        public GameView()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            instance = this;
            ticker.Elapsed += Tick;
        }
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        private void Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Restart();

            Queue<TreeNode<CodePart>> q = new Queue<TreeNode<CodePart>>();
            q.Enqueue(Game.CodeTree);
            while (q.Count > 0)
            {
                TreeNode<CodePart> current = q.Dequeue();
                if (current == null)
                    continue;
                if(current.value.Execute())
                foreach (TreeNode<CodePart> cp in current.Children)
                    q.Enqueue(cp);
            }


            Invalidate();
            timer.Stop();
            var elapsed = timer.ElapsedMilliseconds;
            if (elapsed > 0)
                ticker.Interval = elapsed;
            else ticker.Interval = 1;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            if (Game.Initalized)
                pevent.Graphics.Clear(Game.Palette[int.Parse(BackColor.ToString(), System.Globalization.NumberStyles.HexNumber)]);
            else pevent.Graphics.Clear(Color.Black);
            pevent.Graphics.DrawRectangle(Pens.White, ClientRectangle);

        }
        int posX, posY = 0;
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (Playing)
            {
                TextRenderer.DrawText(pe.Graphics, $"STIMULATING,\tFPS: {(60 / ticker.Interval).ToString("f0")},\tRATE: {ticker.Interval.ToString("f0")}ms", Font, new Point(0, 0), Color.White);
            }
        }
    }
}
