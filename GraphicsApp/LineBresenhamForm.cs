using System;
using System.Drawing;
using System.Windows.Forms;

public class LineBresenhamForm : Form
{
    private Panel drawPanel;
    private Button drawButton;
    private TextBox inputX1, inputY1, inputX2, inputY2;
    private Label labelX1, labelY1, labelX2, labelY2;
    private DataGridView resultTable;
    private const float scale = 5.0f; // Class-level scaling factor

    public LineBresenhamForm()
    {
        Text = "Line Bresenham Algorithm";
        Size = new Size(800, 600);

        drawButton = new Button
        {
            Text = "Draw Line",
            Location = new Point(20, 20),
            Width = 100
        };
        drawButton.Click += DrawButton_Click;

        labelX1 = new Label { Text = "X1:", Location = new Point(100, 20), Width = 30 };
        inputX1 = new TextBox { Location = new Point(130, 20), Width = 60, Text = "16" };
        labelY1 = new Label { Text = "Y1:", Location = new Point(200, 20), Width = 30 };
        inputY1 = new TextBox { Location = new Point(230, 20), Width = 60, Text = "3" };
        labelX2 = new Label { Text = "X2:", Location = new Point(300, 20), Width = 30 };
        inputX2 = new TextBox { Location = new Point(330, 20), Width = 60, Text = "64" };
        labelY2 = new Label { Text = "Y2:", Location = new Point(400, 20), Width = 30 };
        inputY2 = new TextBox { Location = new Point(430, 20), Width = 60, Text = "8" };

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
        Controls.Add(labelX1);
        Controls.Add(inputX1);
        Controls.Add(labelY1);
        Controls.Add(inputY1);
        Controls.Add(labelX2);
        Controls.Add(inputX2);
        Controls.Add(labelY2);
        Controls.Add(inputY2);
        Controls.Add(drawPanel);
        Controls.Add(resultTable);
    }

    private void DrawButton_Click(object sender, EventArgs e)
    {
        if (int.TryParse(inputX1.Text, out int x1) && int.TryParse(inputY1.Text, out int y1) &&
            int.TryParse(inputX2.Text, out int x2) && int.TryParse(inputY2.Text, out int y2))
        {
            drawPanel.Invalidate();
        }
        else
        {
            MessageBox.Show("Please enter valid numbers in all boxes.");
        }
    }

    private void DrawPanel_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        // Draw X and Y axes (shorter length, 80% of panel size)
        int centerX = drawPanel.Width / 2;
        int centerY = drawPanel.Height / 2;
        int axisLength = (int)(drawPanel.Width / 2);
        using (Pen axisPen = new Pen(Color.Black, 2))
        {
            g.DrawLine(axisPen, centerX - axisLength, centerY, centerX + axisLength, centerY);
            g.DrawLine(axisPen, centerX, centerY - axisLength, centerX, centerY + axisLength);
        }

        if (int.TryParse(inputX1.Text, out int x1) && int.TryParse(inputY1.Text, out int y1) &&
            int.TryParse(inputX2.Text, out int x2) && int.TryParse(inputY2.Text, out int y2))
        {
            resultTable.Rows.Clear();
            DrawLineBresenham(g, x1, y1, x2, y2);
        }
    }

    private void DrawLineBresenham(Graphics g, int x0, int y0, int xEnd, int yEnd)
    {
        // Calculate deltas
        int dx = Math.Abs(xEnd - x0);
        int dy = Math.Abs(yEnd - y0);
        int sx = x0 < xEnd ? 1 : -1;
        int sy = y0 < yEnd ? 1 : -1;
        int x = x0;
        int y = y0;
        int p = 2 * dy - dx; // Initial decision parameter p0 = 2Δy - Δx

        int k = 0;
        int centerX = drawPanel.Width / 2;
        int centerY = drawPanel.Height / 2;
        resultTable.Rows.Add(k++, p, $"({x}, {y})");

        // Draw a single straight line between start and end points
        int startX = centerX + (int)(x0 * scale);
        int startY = centerY - (int)(y0 * scale);
        int endX = centerX + (int)(xEnd * scale);
        int endY = centerY - (int)(yEnd * scale);
        g.DrawLine(Pens.Black, startX, startY, endX, endY);

        // Bresenham's algorithm for |m| < 1 to compute points for table
        int steps = Math.Abs(xEnd - x0);
        for (int kStep = 0; kStep < steps; kStep++)
        {
            if (p < 0)
            {
                // Choose lower pixel (East)
                x += sx;
                p += 2 * dy;
            }
            else
            {
                // Choose upper pixel (North East)
                x += sx;
                y += sy;
                p += 2 * (dy - dx);
            }
            resultTable.Rows.Add(k++, p, $"({x}, {y})");
        }
    }

    private void Plot(Graphics g, int x, int y)
    {
        int centerX = drawPanel.Width / 2;
        int centerY = drawPanel.Height / 2;
        g.FillRectangle(Brushes.Black, centerX + (int)(x * scale), centerY - (int)(y * scale), 2, 2);
    }
}