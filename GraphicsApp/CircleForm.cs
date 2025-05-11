using System;
using System.Drawing;
using System.Windows.Forms;

public class CircleForm : Form
{
    private Panel drawPanel;
    private Button drawButton;
    private NumericUpDown inputX, inputY, inputR;
    private DataGridView resultTable;

    public CircleForm()
    {
        Text = "Circle (Midpoint)";
        Size = new Size(800, 600);

        drawButton = new Button
        {
            Text = "Draw Circle",
            Location = new Point(20, 20),
            Width = 100
        };
        drawButton.Click += DrawButton_Click;

        inputX = new NumericUpDown { Location = new Point(130, 20), Width = 60, Minimum = -200, Maximum = 200, Value = 0 };
        inputY = new NumericUpDown { Location = new Point(200, 20), Width = 60, Minimum = -200, Maximum = 200, Value = 0 };
        inputR = new NumericUpDown { Location = new Point(270, 20), Width = 60, Minimum = 0, Maximum = 200, Value = 50 };

        drawPanel = new Panel
        {
            Location = new Point(20, 60),
            Size = new Size(740, 300),
            BackColor = Color.White
        };
        drawPanel.Paint += DrawPanel_Paint;

        resultTable = new DataGridView
        {
            Location = new Point(20, 370),
            Size = new Size(740, 150),
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            ReadOnly = true
        };
        resultTable.Columns.Add("k", "k");
        resultTable.Columns.Add("Pk", "P_k");
        resultTable.Columns.Add("XY", "(X_k, Y_k)");

        Controls.Add(drawButton);
        Controls.Add(inputX);
        Controls.Add(inputY);
        Controls.Add(inputR);
        Controls.Add(drawPanel);
        Controls.Add(resultTable);
    }

    private void DrawButton_Click(object sender, EventArgs e)
    {
        drawPanel.Invalidate();
    }

    private void DrawPanel_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        // Draw X and Y axes (shorter length, 80% of panel size)
        int centerX = drawPanel.Width / 2;
        int centerY = drawPanel.Height / 2;
        int axisLength = (int)(drawPanel.Width * 0.8 / 2);
        using (Pen axisPen = new Pen(Color.Black, 2))
        {
            g.DrawLine(axisPen, centerX - axisLength, centerY, centerX + axisLength, centerY); // X-axis
            g.DrawLine(axisPen, centerX, centerY - axisLength, centerX, centerY + axisLength); // Y-axis
        }

        int xc = (int)inputX.Value;
        int yc = (int)inputY.Value;
        int r = (int)inputR.Value;

        resultTable.Rows.Clear();
        DrawCircle(g, xc, yc, r);
    }

    private void DrawCircle(Graphics g, int xc, int yc, int r)
    {
        int x = 0;
        int y = r;
        int d = 1 - r;
        int k = 0;

        CirclePoints(g, xc, yc, x, y, k++);
        resultTable.Rows.Add(k-1, d, $"({x}, {y})");

        while (y > x)
        {
            if (d < 0)
            {
                d += 2 * x + 3;
            }
            else
            {
                d += 2 * (x - y) + 5;
                y--;
            }
            x++;
            CirclePoints(g, xc, yc, x, y, k++);
            resultTable.Rows.Add(k-1, d, $"({x}, {y})");
        }
    }

    private void CirclePoints(Graphics g, int xc, int yc, int x, int y, int k)
    {
        Plot(g, xc + x, yc + y);
        Plot(g, xc - x, yc + y);
        Plot(g, xc + x, yc - y);
        Plot(g, xc - x, yc - y);
        Plot(g, xc + y, yc + x);
        Plot(g, xc - y, yc + x);
        Plot(g, xc + y, yc - x);
        Plot(g, xc - y, yc - x);
    }

    private void Plot(Graphics g, int x, int y)
    {
        int centerX = drawPanel.Width / 2;
        int centerY = drawPanel.Height / 2;
        g.FillRectangle(Brushes.Black, centerX + x, centerY - y, 2, 2);
    }
}