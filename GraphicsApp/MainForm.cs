using System;
using System.Drawing;
using System.Windows.Forms;

public class MainForm : Form
{
    private ComboBox algorithmSelector;
    private Button runButton;

    public MainForm()
    {
        Text = "2D Graphics Algorithms";
        Size = new Size(400, 200);

        algorithmSelector = new ComboBox
        {
            Location = new Point(20, 20),
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        algorithmSelector.Items.AddRange(new string[]
        {
            "Line DDA", "Line Bresenham", "Circle", "Ellipse", "2D Transformations"
        });
        algorithmSelector.SelectedIndex = 0;

        runButton = new Button
        {
            Text = "Run Algorithm",
            Location = new Point(230, 20),
            Width = 100
        };
        runButton.Click += RunButton_Click;

        Controls.Add(algorithmSelector);
        Controls.Add(runButton);
    }

    private void RunButton_Click(object sender, EventArgs e)
    {
        Form algorithmForm = null;
        switch (algorithmSelector.SelectedItem.ToString())
        {
            case "Line DDA":
                algorithmForm = new LineDDAForm();
                break;
            case "Line Bresenham":
                algorithmForm = new LineBresenhamForm();
                break;
            case "Circle":
                algorithmForm = new CircleForm();
                break;
            case "Ellipse":
                algorithmForm = new EllipseForm();
                break;
            case "2D Transformations":
                algorithmForm = new TransformationForm();
                break;
        }
        if (algorithmForm != null)
        {
            algorithmForm.Show();
        }
    }

    [STAThread]
    public static void Main()
    {
        Application.Run(new MainForm());
    }
}