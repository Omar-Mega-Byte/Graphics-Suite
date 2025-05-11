using System;
using System.Drawing;
using System.Windows.Forms;

public class EllipseForm : Form
{
    private Panel drawPanel;
    private Button drawButton;
    private NumericUpDown inputX, inputY, inputRx, inputRy;
    private DataGridView resultTable;

    public EllipseForm()
    {
        Text = "Ellipse (Midpoint)";
        Size = new Size(800, 600);

        drawButton = new Button
        {
            Text = "Draw Ellipse",
            Location = new Point(20, 20),
            Width = 100
        };
        drawButton.Click += DrawButton_Click;

        inputX = new NumericUpDown { Location = new Point(130, 20), Width = 60, Minimum = -200, Maximum = 200, Value = 0 };
        inputY = new NumericUpDown { Location = new Point(200, 20), Width = 60, Minimum = -200, Maximum = 200, Value = 0 };
        inputRx = new NumericUpDown { Location = new Point(270, 20), Width = 60, Minimum = 0, Maximum = 200, Value = 50 };
        inputRy = new NumericUpDown { Location = new Point(340, 20), Width = 60, Minimum = 0, Maximum = 200, Value = 25 };

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
        resultTable.Columns.Add("P_k", "P_k");
        resultTable.Columns.Add("XY", "(X_k, Y_k)");
        resultTable.Columns.Add("2r_y^2*x_k+1", "2r_y^2*x_k+1");
        resultTable.Columns.Add("2r_x^2*y_k+1", "2r_x^2*y_k+1");

        Controls.Add(drawButton);
        Controls.Add(inputX);
        Controls.Add(inputY);
        Controls.Add(inputRx);
        Controls.Add(inputRy);
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
        int rx = (int)inputRx.Value;
        int ry = (int)inputRy.Value;

        resultTable.Rows.Clear();
        DrawEllipse(g, xc, yc, rx, ry);
    }

    private void DrawEllipse(Graphics g, int xc, int yc, int rx, int ry)
    {
        int x = 0;
        int y = ry;
        int rx2 = rx * rx;
        int ry2 = ry * ry;
        int twoRx2 = 2 * rx2;
        int twoRy2 = 2 * ry2;
        int px = 0;
        int py = twoRx2 * y;

        // Region 1 initial decision parameter
        int p1 = ry2 - rx2 * ry + (int)(0.25 * rx2);
        int k = 0;

        // Plot initial point
        PlotEllipsePoints(g, xc, yc, x, y);
        resultTable.Rows.Add(k++, p1, $"({x}, {y})", px, py);

        // Region 1
        while (px < py)
        {
            x++;
            px += twoRy2;
            if (p1 < 0)
            {
                p1 += px + ry2;
            }
            else
            {
                y--;
                py -= twoRx2;
                p1 += px - py + ry2;
            }
            PlotEllipsePoints(g, xc, yc, x, y);
            resultTable.Rows.Add(k++, p1, $"({x}, {y})", px, py);
        }

        // Region 2 initial decision parameter
        p1 = (int)(ry2 * (x + 0.5) * (x + 0.5) + rx2 * (y - 1) * (y - 1) - rx2 * ry2);

        // Region 2
        while (y >= 0)
        {
            if (p1 > 0)
            {
                y--;
                py -= twoRx2;
                p1 += rx2 - py;
            }
            else
            {
                x++;
                px += twoRy2;
                y--;
                py -= twoRx2;
                p1 += px - py + rx2;
            }
            PlotEllipsePoints(g, xc, yc, x, y);
            resultTable.Rows.Add(k++, p1, $"({x}, {y})", px, py);
        }
    }

    private void PlotEllipsePoints(Graphics g, int xc, int yc, int x, int y)
    {
        Plot(g, xc + x, yc + y);
        Plot(g, xc - x, yc + y);
        Plot(g, xc + x, yc - y);
        Plot(g, xc - x, yc - y);
    }

    private void Plot(Graphics g, int x, int y)
    {
        int centerX = drawPanel.Width / 2;
        int centerY = drawPanel.Height / 2;
        g.FillRectangle(Brushes.Black, centerX + x, centerY - y, 2, 2);
    }
}