using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    public class Level_problem
    {
        public int N;
        public int M;
        public int start;
        public int dest;
        public String answer;
        public String hint;
        public Level_problem(int n, int m, int s, int d, String a, String h) { N = n; M = m; start = s; dest = d; answer = a; hint = h; }
    };
    
public class Direction{
	public int x;
	public int y;
	public double l;
	public Direction(int xx,int yy){x=xx;y=yy;l=Math.Sqrt(xx*xx+yy*yy);}
};
public class Node{
	public int x;
	public int y;
	public int father;
	public double length;
	public Node(int xx,int yy,int f,double len){x=xx;y=yy;father=f;length=len;}
};

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Level_problems.Add(new Level_problem(2, 3, 1, 6, "7.893","Start from the red, end in the green."));
            Level_problems.Add(new Level_problem(2, 3, 2, 5, "8.886","Pass by the spots already passed."));
            Level_problems.Add(new Level_problem(3, 3, 1, 9, "15.951","Not so hard huh..."));
            Level_problems.Add(new Level_problem(3, 3, 5, 1, "17.365","Feel challenged?"));
            Level_problems.Add(new Level_problem(3, 3, 5, 0, "17.779","You can end in anywhere."));
            Level_problems.Add(new Level_problem(3, 4, 1, 6, "28.043","Call for help."));
            Level_problems.Add(new Level_problem(3, 4, 6, 0, "29.211","Even Jack has to think for quite a while."));
            load_level(0);
            

        }
        private delegate void RefreshUI();
        private RefreshUI answer;
        private void refresh()
        {
            TextBlock_help.Text = route;
        }
        List<Level_problem> Level_problems = new List<Level_problem>();
        List<Node> nodeList=new List<Node>();
        String route;
        int level_number=0;
        int N,M,start,dest,start_x,start_y,dest_x,dest_y;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
           // answer = new RefreshUI(refresh);
            //Thread t = new Thread(calculate);
            //t.Start();
            calculate();
            max_level++;
        }

        private void calculate() {
            nodeList.Clear();
            int total = N * M;
            List<Direction>[,] dirs = new List<Direction>[N, M];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    dirs[i, j] = new List<Direction>();
                    for (int ii = 0; ii < N; ii++)
                    {
                        for (int jj = 0; jj < M; jj++)
                        {
                            int x = ii - i, y = jj - j;
                            if (x != 0 || y != 0)
                            {
                                int c = 1;
                                if (x == 0) c = Math.Abs(y);
                                else if (y == 0) c = Math.Abs(x);
                                else
                                {
                                    for (int cc = 1; cc <= Math.Min(Math.Abs(x), Math.Abs(y)); cc++)
                                    {
                                        if (x % cc == 0 && y % cc == 0)
                                        {
                                            c = cc;
                                        }
                                    }
                                }
                                x /= c; y /= c;
                                bool exist = false;
                                for (int index = 0; index < dirs[i, j].Count; index++)
                                {
                                    if (dirs[i, j][index].x == x && dirs[i, j][index].y == y)
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                                if (exist == false)
                                {
                                    dirs[i, j].Add(new Direction(x, y));
                                }
                            }
                        }
                    }
                }
            }
            nodeList = new List<Node>();
            Queue<int> open = new Queue<int>();
            bool visited;
            int nodeNumber = 0, level = 1, father, temp;
            double max = 0;
            open.Enqueue(nodeNumber);
            nodeList.Add(new Node(start_x, start_y, -1, 0.0));
            nodeNumber++;
            level++;
            open.Enqueue(-2);
            while (open.Count != 0)
            {
                temp = open.Dequeue();
                if (temp >= 0)
                {
                    int x = nodeList[temp].x, y = nodeList[temp].y;
                    for (int i = 0; i < dirs[x, y].Count; i++)
                    {

                        for (int c = 1, 
                            next_x = nodeList[temp].x + dirs[x, y][i].x, 
                            next_y = nodeList[temp].y + dirs[x, y][i].y; 
                            next_x < N && next_x >= 0 && next_y < M && next_y >= 0; 
                            next_x += dirs[x, y][i].x, 
                            next_y += dirs[x, y][i].y,
                            c++)
                        {
                            visited = false;

                            if (dest!=0 && level!=N*M && next_x == dest_x && next_y == dest_y)
                            {
                                break;
                            }
                            else
                            {
                                father = temp;
                                while (father != -1)
                                {
                                    if (nodeList[father].x == next_x && nodeList[father].y == next_y)
                                    {
                                        visited = true;
                                        break;
                                    }
                                    father = nodeList[father].father;
                                }
                            }
                            if (!visited)
                            {
                                if (level < total)
                                {
                                    nodeList.Add(new Node(next_x, next_y, temp, nodeList[temp].length + c * dirs[x, y][i].l));
                                    open.Enqueue(nodeNumber);
                                    nodeNumber++;
                                    break;
                                }
                                else
                                {
                                    double route_l=nodeList[temp].length + c * dirs[x, y][i].l;
                                    if (route_l > max)
                                    {
                                        nodeList.Add(new Node(next_x, next_y, temp, route_l));
                                        nodeNumber++;
                                        max = route_l;
                                        break;
                                    }

                                }

                            }
                        }

                    }
                    //open.push(-1);
                }

                //else if(temp==-1){
                //cout<<" ";
                //}
                else if (temp == -2)
                {
                    if (open.Count != 0)
                    {
                        level++;
                        open.Enqueue(-2);
                    }
                }
            }
            one_solution.Clear();
           	int f=nodeList.Count-1;
            one_solution.Add(f);
            //route = "(" + (nodeList[f].x + 1).ToString() + "," + (nodeList[f].y + 1).ToString()+")";
            f = nodeList[f].father;
            while(f!=-1){
                one_solution.Add(f);
                //route = "(" + (nodeList[f].x + 1).ToString() + "," + (nodeList[f].y + 1).ToString() + ")" + "\r\n" + route;
                f=nodeList[f].father;
            }
            route = "max length is "+nodeList[nodeList.Count - 1].length.ToString("0.000")+"\r\nCLICK HERE to watch every step";
            Lines.Children.Clear();
            Lines.Children.Add(new LineGeometry(new Point(0, 0), new Point(0, 0)));
            path.Data = Lines;
            step = one_solution.Count - 1;
            refresh();
        }
        private List<int> one_solution = new List<int>();
        int max_level=0;
        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_help.Text = "";
            if (level_number<max_level)
            {

                if (level_number < Level_problems.Count - 1)
                {
                    level_number++;
                    max_level = level_number;
                    load_level(level_number);
                    Lines.Children.Clear();
                }
            }
            else { 
                TextBlock_help.Text = "Please pass this level first!";
                
            }
        }

        private void Button_Previous_Click(object sender, RoutedEventArgs e)
        {

            if (level_number > 0)
            {
                level_number--;
                load_level(level_number);
                
            }
        }
        //List<Ellipse>[] maps = new List<Ellipse>[4];
        SolidColorBrush black= new SolidColorBrush(Colors.Black);
        SolidColorBrush white = new SolidColorBrush(Colors.White);
        SolidColorBrush green = new SolidColorBrush(Colors.Green);
        SolidColorBrush red = new SolidColorBrush(Colors.Red);
        SolidColorBrush aqua = new SolidColorBrush(Colors.Aqua);
        
        SolidColorBrush back = new SolidColorBrush(Color.FromRgb(244, 244, 245));

        LineGeometry initialize = new LineGeometry(new Point(0, 0), new Point(0, 0));
        void load_level(int n)
        {
            TextBlock_hint.Text = Level_problems[level_number].hint;
            M = Level_problems[n].M;
            N = Level_problems[n].N;
            start = Level_problems[n].start;
            dest = Level_problems[n].dest;
            
            start_x = (start - 1) / M;
            start_y = (start - 1) % M;
            dest_x = (dest - 1) / M;
            dest_y = (dest - 1) % M;
            TextBlock_Level_number.Text = "Level " + (n + 1).ToString();
            c1.Fill = c1.Stroke = white;
            c2.Fill = c2.Stroke = white;
            c3.Fill = c3.Stroke = white;
            c4.Fill = c4.Stroke = white;
            c5.Fill = c5.Stroke = white;
            c6.Fill = c6.Stroke = white;
            c7.Fill = c7.Stroke = white;
            c8.Fill = c8.Stroke = white;
            c9.Fill = c9.Stroke = white;
            c10.Fill = c10.Stroke = white;
            c11.Fill = c11.Stroke = white;
            c12.Fill = c12.Stroke = white;
            Lines.Children.Clear();
            Lines.Children.Add(initialize);
            path.Data = Lines;
            path.Stroke = white;
            if (M * N == 6){
                c1.Fill=back; c1.Stroke=black;
                c2.Fill=back; c2.Stroke=black;
                c3.Fill=back; c3.Stroke=black;
                c5.Fill=back; c5.Stroke=black;
                c6.Fill=back; c6.Stroke=black;
                c7.Fill=back; c7.Stroke=black;
            }
            else if (M * N == 9){
                c1.Fill=back; c1.Stroke=black;
                c2.Fill=back; c2.Stroke=black;
                c3.Fill=back; c3.Stroke=black;
                c5.Fill=back; c5.Stroke=black;
                c6.Fill=back; c6.Stroke=black;
                c7.Fill=back; c7.Stroke=black;
                c9.Fill=back; c9.Stroke=black;
                c10.Fill=back; c10.Stroke=black;
                c11.Fill=back; c11.Stroke=black;
            }
            else if (M * N == 12) {
                c1.Fill=back; c1.Stroke=black;
                c2.Fill=back; c2.Stroke=black;
                c3.Fill=back; c3.Stroke=black;
                c4.Fill=back; c4.Stroke=black;
                c5.Fill=back; c5.Stroke=black;
                c6.Fill=back; c6.Stroke=black;
                c7.Fill=back; c7.Stroke=black;
                c8.Fill=back; c8.Stroke=black;
                c9.Fill=back; c9.Stroke=black;
                c10.Fill=back; c10.Stroke=black;
                c11.Fill=back; c11.Stroke=black;
                c12.Fill=back; c12.Stroke=black;
            }
            switch (level_number)
            {
                case 0:
                    c1.Stroke =red;
                    c7.Stroke=green;
                    break;
                case 1: 
                    c2.Stroke =red;
                    c6.Stroke=green;
                    break;
                case 2: 
                    c1.Stroke =red;
                    c11.Stroke=green;
                    break;
                case 3:
                    c6.Stroke =red;
                    c1.Stroke=green;
                    break;
                case 4: 
                    c6.Stroke = red;
                    break;
                case 5: 
                    c1.Stroke = red;
                    c6.Stroke = green;
                    break;
                case 6:
                    c6.Stroke = red;
                    break;
            }
            
        }

        bool drawing = false;
        
        private List<Node> visitedNodes = new List<Node>();
        private GeometryGroup Lines=new GeometryGroup();
        private void link(int x, int y)
        {
            if (drawing)
            {
                bool visited = false;
                double plusLength = 0;
                int last = visitedNodes.Count - 1;
                int pre_axis_x, pre_axis_y,axis_x,axis_y;
                for (int i = last; i >=0; i--)
                {
                    if (visitedNodes[i].x == x && visitedNodes[i].y == y)
                    {
                        visited = true;
                        break;
                    }
                }
                
                if (!visited)
                {
                  
                    if (last == -1) { 
                        visitedNodes.Add(new Node(x, y, last, 0));
                        //TextBlock_help.Text += "(" + (x + 1) + "," + (y + 1) + ")\r\n";
                        
                    }
                    else
                    {
                        pre_axis_x = 10 + 60 * (visitedNodes[last].x);
                        pre_axis_y = 10 + 60 * (visitedNodes[last].y);
                
                        plusLength = Math.Sqrt((x - visitedNodes[last].x) * (x - visitedNodes[last].x) + (y - visitedNodes[last].y) * (y - visitedNodes[last].y));
                        visitedNodes.Add(new Node(x, y, last, visitedNodes[last].length + plusLength));
                        
                        axis_x = 10 + 60 * x ;
                        axis_y = 10 + 60 * y ;

                        //TextBlock_help.Text += "(" + (x+1) + "," + (y+1) + ")\r\n";
                        Lines.Children.Add(new LineGeometry(new Point(pre_axis_y, pre_axis_x), new Point(axis_y, axis_x)));                      
                        path.Data = Lines;
                        path.Stroke = aqua;
                        TextBlock_help.Text = "current length is " + (visitedNodes[last].length + plusLength).ToString("0.000");
                    }
                
                }

                
            
            }
        }
        
        private void c1_MouseMove(object sender, MouseEventArgs e)
        {
            //Lines.Children.Add(new LineGeometry(new Point(0,0),new Point(e.GetPosition(Canvas_map).X,e.GetPosition(Canvas_map).Y)));
            //path.Stroke = aqua;
            if (e.LeftButton.ToString() == "Pressed")
            {
                link(0, 0);
            }
        }

        private void c2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed")
            {
                link(0, 1);
            }

        }
        private void c3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed")
            {
                link(0, 2);
            }

        }
        private void c4_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed" && M * N ==12)
            {
                link(0, 3);
            }

        }
        private void c5_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed")
            {
                link(1, 0);
            }

        }
        private void c6_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed")
            {
                link(1, 1);
            }

        }
        private void c7_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed")
            {
                link(1, 2);
            }

        }
        private void c8_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed" && M*N==12)
            {
                link(1, 3);
            }

        }
        private void c9_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed" && M * N != 6)
            {
                link(2, 0);
            }

        }
        private void c10_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed" && M * N != 6)
            {
                link(2, 1);
            }

        
        }

        private void c11_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed" && M * N != 6)
            {
                link(2, 2);
            }

        }
        private void c12_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton.ToString() == "Pressed" && M * N ==12)
            {
                link(2, 3);
            
            }

        }
        private void Canvas_map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drawing = true;
            TextBlock_help.Text = "";
            Lines.Children.Clear();
            Lines.Children.Add(initialize);
            visitedNodes.Clear();
        }

        private void Canvas_map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drawing = false;
            int last=visitedNodes.Count-1;
            if (last >= 0)
            {
                check_answer();
                
            }
        }

        private void check_answer()
        {
            if (Level_problems[level_number].answer == visitedNodes[visitedNodes.Count - 1].length.ToString("0.000"))
            {
                TextBlock_help.Text = "Unlocked.";
             
                max_level++;
            }
            else
            {
                TextBlock_help.Text = "Nein...\r\nTry again!";
                Lines.Children.Clear();
                path.Data = Lines;
                path.Stroke = white;
                Lines.Children.Add(initialize);
               
            }
            visitedNodes.Clear();
                
        }
        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            if (visitedNodes.Count > 0 && e.LeftButton.ToString() != "Pressed")
            {
                check_answer();
            }
        }

        int step;
        
        private void Text_route_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (step > 0)
            {
                Lines.Children.Add(new LineGeometry(new Point(10 + 60 * nodeList[one_solution[step]].y, 10 + 60 * nodeList[one_solution[step]].x),
               new Point(10 + 60 * nodeList[one_solution[step - 1]].y, 10 + 60 * nodeList[one_solution[step - 1]].x)));
                path.Data = Lines;
                path.Stroke = aqua;
                step--;

            }

        }

    }
}
        