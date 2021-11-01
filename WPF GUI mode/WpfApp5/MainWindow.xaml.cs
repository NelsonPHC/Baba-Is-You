using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace WpfApp5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            level_index = 0;
            storeLevels();
            InitializeComponent();
            initWorld();
            buildWorld();

        }
        private const int OFFSET = 0;   // offset of the top left corner of view
        private const int SPACE = 20;   // distance for one step (= image size)
        private const int LEFT_COLLISION = 1;
        private const int RIGHT_COLLISION = 2;
        private const int TOP_COLLISION = 3;
        private const int BOTTOM_COLLISION = 4;


        private int w = 0;  // the width of the level (for view)
        private int h = 0;  // the height of the level

        private List<Wall> walls;
        private List<Baggage> baggs;    // rocks & word blocks
        private List<Area> areas; // may have mulitple areas  ( _area_ is win)
        private List<GOOP> goops;
        private Player soko;
        private Boolean iscompleted = false;
        private Boolean isdead = false;

        Originator originator;
        CareTaker careTaker;

        private List<String> levels;
        private String currentLevel;

        
        private String level;
        private void storeLevels()
        {
            levels = new List<string>();

            level
            = "#########################\n"
            + "#Rw#                    #\n"
            + "#II#         #          #\n"
            + "#PS#          # .       #\n"
            + "####         #          #\n"
            + "#             # #       #\n"
            + "#   BIU     #$ # #      #\n"
            + "#           # F W       #\n"
            + "#     @   #  # #        #\n"
            + "#                       #\n"
            + "#                       #\n"
            + "#########################\n";

            levels.Add(level);

            level
            = "#########################\n"
            + "#Bw#                    #\n"
            + "#II#        GIs         #\n"
            + "#US#      ##########    #\n"
            + "####      #        #    #\n"
            + "#         #  RIP   #    #\n"
            + "#         #        #    #\n"
            + "#         # @  $$  #    #\n"
            + "#         #        #    #\n"
            + "#############****########\n"
            + "#                       #\n"
            + "#                       #\n"
            + "#***                    #\n"
            + "#***          FIW       #\n"
            + "#.**                    #\n"
            + "#########################\n";

            levels.Add(level);
        }

        private int level_index;
        private void initWorld()
        {
            
            walls = new List<Wall>();
            baggs = new List<Baggage>();
            areas = new List<Area>();
            goops = new List<GOOP>();
            
            originator = new Originator();
            careTaker = new CareTaker();
            

            int x = OFFSET;
            int y = OFFSET;
            Wall wall;
            Baggage b;
            Area a;
            GOOP g;

            

            currentLevel = levels[level_index];

            for (int i = 0; i < currentLevel.Length; i++)
            {

                char item = currentLevel[i];

                switch (item)
                {

                    case '\n':
                        y += SPACE;

                        if (w < x) // update the leftmost position as width
                        {
                            w = x;
                        }

                        x = OFFSET;
                        break;

                    case '#':
                        wall = new Wall(x, y);
                        walls.Add(wall);
                        x += SPACE;
                        break;

                    case '$':
                        b = new Baggage(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case '*':
                        g = new GOOP(x, y);
                        goops.Add(g);
                        x += SPACE;
                        break;
                    
                    case '.':
                        a = new Area(x, y);
                        areas.Add(a);
                        x += SPACE;
                        break;

                    case '@':
                        soko = new Player(x, y);
                        x += SPACE;
                        break;

                    case ' ':
                        x += SPACE;
                        break;

                    // Word blocks
                    case 'B':
                        b = new BABA(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'R':
                        b = new ROCK(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'I':
                        b = new IS(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'P':
                        b = new PUSH(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'U':
                        b = new YOU(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'F':
                        b = new FLAG(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'W':
                        b = new WIN(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'w':
                        b = new WALL(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'S':
                        b = new STOP(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 's':
                        b = new SINK(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    case 'G':
                        b = new Goop(x, y);
                        baggs.Add(b);
                        x += SPACE;
                        break;

                    default:
                        break;
                }

                h = y;  // get the bottom height
            }
            saveState();
        }

        

        private void buildWorld()
        {
            //the view
            foreach (Wall w in walls)
            {
                
                w.draw((MainWindow)Application.Current.MainWindow);
            }
            foreach (Baggage b in baggs)
            {
                b.draw((MainWindow)Application.Current.MainWindow);

            }
            foreach(GOOP g in goops)
            {
                g.draw((MainWindow)Application.Current.MainWindow);
            }

            foreach (Area area in areas)
            {
                area.draw((MainWindow)Application.Current.MainWindow);
            }

            soko.draw((MainWindow)Application.Current.MainWindow);

        }

        private void update()
        {
            // for update of the display

            canvas2d.Children.Clear();


            foreach (Wall w in walls)
            {
                w.draw((MainWindow)Application.Current.MainWindow);
            }
            foreach (Baggage b in baggs)
            {
                b.draw((MainWindow)Application.Current.MainWindow);

            }
            foreach (GOOP g in goops)
            {
                g.draw((MainWindow)Application.Current.MainWindow);
            }

            foreach (Area area in areas)
            {
                area.draw((MainWindow)Application.Current.MainWindow);
            }

            soko.draw((MainWindow)Application.Current.MainWindow);
            if (iscompleted == true)
            {
                if(level_index == levels.Count - 1)
                    MessageBox.Show("You won all levels! Congrates!");
                else
                {
                    if(MessageBox.Show("You won! Click ok to proceed to the next level.") == MessageBoxResult.OK)
                        nextLevel();
                }
            }

            if (isdead == true)
            {
                MessageBox.Show("You died. Press R to restart level.");
            }



        }



        private Boolean checkWallCollision(Actor actor, int type)
        {

            switch (type)
            {

                case LEFT_COLLISION:

                    for (int i = 0; i < walls.Count; i++)
                    {

                        Wall wall = walls[i];

                        if (actor.isLeftCollision(wall))
                        {
                            return true;
                        }
                    }

                    return false;

                case RIGHT_COLLISION:

                    for (int i = 0; i < walls.Count; i++)
                    {

                        Wall wall = walls[i];

                        if (actor.isRightCollision(wall))
                        {
                            return true;
                        }
                    }

                    return false;

                case TOP_COLLISION:

                    for (int i = 0; i < walls.Count; i++)
                    {

                        Wall wall = walls[i];

                        if (actor.isTopCollision(wall))
                        {

                            return true;
                        }
                    }

                    return false;

                case BOTTOM_COLLISION:

                    for (int i = 0; i < walls.Count; i++)
                    {

                        Wall wall = walls[i];

                        if (actor.isBottomCollision(wall))
                        {

                            return true;
                        }
                    }

                    return false;

                default:
                    break;
            }

            return false;
        }

        private Boolean checkBagCollision(Actor actor, int type)
        {
            List<Baggage> movableBaggs;
            if (!RockIsPush())
            {
                movableBaggs = wordblocks;
            }
            else
            {
                movableBaggs = baggs;
            }
            
            //List<Baggage> itemsToBeMoved = new List<Baggage>();
            switch (type)
            {

                case LEFT_COLLISION:

                    foreach(Baggage bag in movableBaggs)
                    {
                        if (actor.isLeftCollision(bag))
                        {
                            foreach(Baggage item in movableBaggs)
                            {                               
                                if (bag.isLeftCollision(item))
                                {
                                    if (checkWallCollision(item, LEFT_COLLISION) || checkWallCollision(actor, LEFT_COLLISION))
                                        return true;

                                    checkBagCollision(item, LEFT_COLLISION);
                                    item.move(-SPACE, 0);
                                }
                                

                                if (checkWallCollision(bag, LEFT_COLLISION))
                                {
                                    return true;
                                }
                            }
                            
                            bag.move(-SPACE, 0);                            
                        }
                    }

                    return false;


                case RIGHT_COLLISION:

                    foreach (Baggage bag in movableBaggs)
                    {
                        if (actor.isRightCollision(bag))
                        {
                            foreach (Baggage item in movableBaggs)
                            {
                                if (bag.isRightCollision(item))
                                {
                                    if (checkWallCollision(item, RIGHT_COLLISION) || checkWallCollision(actor, RIGHT_COLLISION))
                                        return true;

                                    checkBagCollision(item, RIGHT_COLLISION);
                                    item.move(SPACE, 0);
                                }


                                if (checkWallCollision(bag, RIGHT_COLLISION))
                                {
                                    return true;
                                }
                            }

                            bag.move(SPACE, 0);
                        }
                    }

                    return false;

                case TOP_COLLISION:

                    foreach (Baggage bag in movableBaggs)
                    {
                        if (actor.isTopCollision(bag))
                        {
                            foreach (Baggage item in movableBaggs)
                            {
                                if (bag.isTopCollision(item))
                                {
                                    if (checkWallCollision(item, TOP_COLLISION) || checkWallCollision(actor, TOP_COLLISION))
                                        return true;

                                    checkBagCollision(item, TOP_COLLISION);
                                    item.move(0, -SPACE);
                                }


                                if (checkWallCollision(bag, TOP_COLLISION))
                                {
                                    return true;
                                }
                            }

                            bag.move(0, -SPACE);
                        }
                    }

                    return false;

                case BOTTOM_COLLISION:

                    foreach (Baggage bag in movableBaggs)
                    {
                        if (actor.isBottomCollision(bag))
                        {
                            foreach (Baggage item in movableBaggs)
                            {
                                if (bag.isBottomCollision(item))
                                {
                                    if (checkWallCollision(item, BOTTOM_COLLISION) || checkWallCollision(actor, BOTTOM_COLLISION))
                                        return true;

                                    checkBagCollision(item, BOTTOM_COLLISION);
                                    item.move(0, SPACE);
                                }


                                if (checkWallCollision(bag, BOTTOM_COLLISION))
                                {
                                    return true;
                                }
                            }

                            bag.move(0, SPACE);
                        }
                    }

                    return false;

                default:
                    break;
            }

            return false;
        }

        public void isCompleted()
        {
            if (FlagIsWin()||BABAIsWin())
            {
                foreach (Area area in areas)
                {
                    if (soko.X == area.X && soko.Y == area.Y) // if player reached any of the areas
                    {
                        iscompleted = true;
                    }
                }
            }
            else if (RockIsWin())
            {
                foreach (Baggage b in baggs)
                {
                    if (b.GetType().IsAssignableFrom(typeof(Baggage)))
                    {
                        if (soko.X == b.X && soko.Y == b.Y)
                        {
                            iscompleted = true;
                        }
                    }
                    
                }
            }
        }

        public void isDead()
        {
            foreach(GOOP g in goops)
            {
                if (soko.X == g.X && soko.Y == g.Y) // if player reached any of the goops
                {
                    isdead = true;
                }
            }
                
        }

        private Baggage cRock;
        private GOOP cGoop;
        public void rockGoopCombo()
        {
            
            foreach (Baggage b in baggs)
            {
                if (b.GetType().IsAssignableFrom(typeof(Baggage)))
                {
                    foreach(GOOP g in goops)
                        if(b.X == g.X && b.Y == g.Y)
                        {
                            cRock = b;
                            cGoop = g;
                        }
                            
                }
            }
            goops.Remove(cGoop);
            baggs.Remove(cRock);
            
        }


        private Baggage baba;
        private List<Baggage> iss;
        private Baggage you;
        private Baggage flag;
        private Baggage win;
        private Baggage rock;
        private Baggage push;
        private List<Baggage> wordblocks;

        public void storeWords()
        {
            iss = new List<Baggage>();
            wordblocks = new List<Baggage>();
            foreach (Baggage b in baggs)
            {
                if (!b.GetType().IsAssignableFrom(typeof(Baggage)))
                {
                    if (b.GetType() == typeof(BABA))
                    {
                        baba = b;
                    }
                    else if (b.GetType() == typeof(YOU))
                    {
                        you = b;
                    }
                    else if (b.GetType() == typeof(FLAG))
                    {
                        flag = b;
                    }
                    else if (b.GetType() == typeof(WIN))
                    {
                        win = b;
                    }
                    else if (b.GetType() == typeof(ROCK))
                    {
                        rock = b;
                    }
                    else if (b.GetType() == typeof(PUSH))
                    {
                        push = b;
                    }
                    else if (b.GetType() == typeof(IS))
                    {
                        iss.Add(b);
                    }

                    wordblocks.Add(b);
                }
                
            }
        }

        public Boolean FlagIsU()
        {
            
            foreach (Baggage i in iss)
                if ((flag.isRightCollision(i) && i.isRightCollision(you))|| (flag.isBottomCollision(i) && i.isBottomCollision(you)))
                    return true;

            return false;
            
        }

        public Boolean BABAIsU()
        {
            
            foreach (Baggage i in iss)
                if ((baba.isRightCollision(i) && i.isRightCollision(you))|| (baba.isBottomCollision(i) && i.isBottomCollision(you)))
                    return true;

            return false;

        }

        public Boolean FlagIsWin()
        {

            foreach (Baggage i in iss)
                if ((flag.isRightCollision(i) && i.isRightCollision(win))|| (flag.isBottomCollision(i) && i.isBottomCollision(win)))
                    return true;

            return false;
        }

        

        public Boolean BABAIsWin()
        {

            foreach (Baggage i in iss)
                if ((baba.isRightCollision(i) && i.isRightCollision(win)) || (baba.isBottomCollision(i) && i.isBottomCollision(win)))
                    return true;

            return false;
        }

        public Boolean RockIsWin()
        {

            foreach (Baggage i in iss)
                if ((rock.isRightCollision(i) && i.isRightCollision(win)) || (rock.isBottomCollision(i) && i.isBottomCollision(win)))
                    return true;

            return false;
        }

        public Boolean RockIsPush()
        {

            foreach (Baggage i in iss)
                if ((rock.isRightCollision(i) && i.isRightCollision(push)) || (rock.isBottomCollision(i) && i.isBottomCollision(push)))
                    return true;

            return false;
        }

        public void restartLevel()
        {

            areas.Clear();
            baggs.Clear();
            walls.Clear();

            initWorld();

            if (iscompleted)
            {
                iscompleted = false;
            }

            if (isdead)
            {
                isdead = false;
            }

        }

        public void nextLevel()
        {
            level_index = (level_index + 1) % levels.Count; // increase level without index out of range
            areas.Clear();
            baggs.Clear();
            walls.Clear();

            initWorld();

            if (iscompleted)
            {
                iscompleted = false;
            }
            update();
        }

        public void saveState()
        {
            State state = new State(walls, baggs, areas, goops, soko);
            originator.setState(state);
            careTaker.add(originator.saveStateToMemento());

        }


        public void undo()
        {
            careTaker.Undo();
            originator.getStateFromMemento(careTaker.getLastMemento());

            State s = originator.getState();
            
            walls = s.getWallState;
            baggs = s.getBagState;
            areas = s.getAreaState;
            goops = s.getGoopState;
            soko = s.getSokoState;
            
        }

        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
            storeWords();

            if (e.Key == Key.R)
                restartLevel();
            else if (isdead)
                return;
            else if (iscompleted)
                return;
            else if (FlagIsU())
            {
                switch (e.Key)
                {
                    case Key.Left:

                        if (checkWallCollision(areas[0], LEFT_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(areas[0], LEFT_COLLISION))
                        {
                            return;
                        }

                        areas[0].move(-SPACE, 0);
                        saveState();
                        break;

                    case Key.Right:

                        if (checkWallCollision(areas[0], RIGHT_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(areas[0], RIGHT_COLLISION))
                        {
                            return;
                        }

                        areas[0].move(SPACE, 0);
                        saveState();
                        break;

                    case Key.Up:

                        if (checkWallCollision(areas[0], TOP_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(areas[0], TOP_COLLISION))
                        {
                            return;
                        }

                        areas[0].move(0, -SPACE);
                        saveState();
                        break;

                    case Key.Down:

                        if (checkWallCollision(areas[0], BOTTOM_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(areas[0], BOTTOM_COLLISION))
                        {
                            return;
                        }

                        areas[0].move(0, SPACE);
                        saveState();
                        break;

                    case Key.Z:

                        undo();

                        break;

                    default:
                        break;

                }
            }
            else if (BABAIsU())
            {
                switch (e.Key)
                {
                    case Key.Left:

                        if (checkWallCollision(soko, LEFT_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(soko, LEFT_COLLISION))
                        {
                            return;
                        }

                        soko.move(-SPACE, 0);
                        saveState();
                        break;

                    case Key.Right:

                        if (checkWallCollision(soko, RIGHT_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(soko, RIGHT_COLLISION))
                        {
                            return;
                        }

                        soko.move(SPACE, 0);
                        saveState();
                        break;

                    case Key.Up:

                        if (checkWallCollision(soko, TOP_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(soko, TOP_COLLISION))
                        {
                            return;
                        }

                        soko.move(0, -SPACE);
                        saveState();
                        break;

                    case Key.Down:

                        if (checkWallCollision(soko, BOTTOM_COLLISION))
                        {
                            return;
                        }

                        if (checkBagCollision(soko, BOTTOM_COLLISION))
                        {
                            return;
                        }

                        soko.move(0, SPACE);
                        saveState();
                        break;

                    case Key.Z:

                        undo();

                        break;

                    default:
                        break;

                }
            }
            isCompleted();
            isDead();
            rockGoopCombo();
            update();
        }

        
    }

    public class Actor
    {

        private static int SPACE = 20;

        protected int x;
        protected int y;

        public Actor(int x, int y)
        {

            this.x = x;
            this.y = y;
        }

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
        }

        protected void setX(int x)
        {
            this.x = x;
        }

        protected void setY(int y)
        {
            this.y = y;
        }

        public void move(int x, int y)
        {

            int dx = X + x;
            int dy = Y + y;

            setX(dx);
            setY(dy);
        }

        public Boolean isLeftCollision(Actor actor)
        {
            return X - SPACE == actor.X && Y == actor.Y;
        }

        public Boolean isRightCollision(Actor actor)
        {
            return X + SPACE == actor.X && Y == actor.Y;
        }

        public Boolean isTopCollision(Actor actor)
        {

            return Y - SPACE == actor.Y && X == actor.X;
        }

        public Boolean isBottomCollision(Actor actor)
        {

            return Y + SPACE == actor.Y && X == actor.X;
        }
    }

    public class Wall : Actor
    {
        
        public Rectangle wallcell;
        public Wall(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initWall();
        }

        private void initWall()
        {
            wallcell = new Rectangle();

            wallcell.Width = 20;
            wallcell.Height = 20;
            wallcell.StrokeThickness = 2;
            wallcell.Stroke = Brushes.Black;
            wallcell.Fill = Brushes.Gray;
        }
        public void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(wallcell);
            Canvas.SetLeft(wallcell, X);
            Canvas.SetTop(wallcell, Y);
        }
    }

    public class Player : Actor
    {
        Ellipse p;
        public Player(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            initPlayer();
        }

        private void initPlayer()
        {
            p = new Ellipse();

            p.Width = 20;
            p.Height = 20;
            p.StrokeThickness = 2;
            p.Stroke = Brushes.Black;
            p.Fill = Brushes.White;
            
        }

        public void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(p);
            Canvas.SetLeft(p, X);
            Canvas.SetTop(p, Y);
        }

    }

    public class Baggage : Actor  // derive word blocks from this class
    {
        Ellipse bagcell;
        public Baggage(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            initBaggage();
        }

        private void initBaggage()
        {
            bagcell = new Ellipse();

            bagcell.Width = 20;
            bagcell.Height = 20;
            bagcell.StrokeThickness = 2;
            bagcell.Stroke = Brushes.Black;
            bagcell.Fill = Brushes.SaddleBrown;
        }

        public virtual void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(bagcell);
            Canvas.SetLeft(bagcell, X);
            Canvas.SetTop(bagcell, Y);
        }

    }

    public class GOOP : Actor
    {
        Rectangle goop;
        public GOOP(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            initGOOP();
        }

        private void initGOOP()
        {
            goop = new Rectangle();

            goop.Width = 20;
            goop.Height = 20;
            goop.StrokeThickness = 2;
            goop.Stroke = Brushes.Black;
            goop.Fill = Brushes.Blue;
        }

        public virtual void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(goop);
            Canvas.SetLeft(goop, X);
            Canvas.SetTop(goop, Y);
        }



    }



    public class Area : Actor
    {
        Rectangle a;
        public Area(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            initArea();
        }

        private void initArea()
        {
            a = new Rectangle();

            a.Width = 20;
            a.Height = 20;
            a.StrokeThickness = 2;
            a.Stroke = Brushes.Black;
            a.Fill = Brushes.Yellow;
        }

        public void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(a);
            Canvas.SetLeft(a, X);
            Canvas.SetTop(a, Y);
        }

    }

    // word blocks that are derived from Baggage class
    public class BABA : Baggage
    {
        TextBlock baba;
        public BABA(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initBABA();
        }

        private void initBABA()
        {
            baba = new TextBlock();
            baba.Width = 20;
            baba.Height = 20;
            baba.Text = "B A\r\nB A";
            baba.FontSize = 7;
            baba.TextAlignment = TextAlignment.Center;
            baba.FontWeight = FontWeights.Bold;
            baba.Background = Brushes.Black;
            baba.Foreground = Brushes.White;
        }
        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(baba);
            Canvas.SetLeft(baba, X);
            Canvas.SetTop(baba, Y);
        }
    }

    public class ROCK : Baggage
    {
        TextBlock rock;
        public ROCK(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initROCK();
        }

        private void initROCK()
        {
            rock = new TextBlock();
            rock.Width = 20;
            rock.Height = 20;
            rock.Text = "R O\r\nC K";
            rock.FontSize = 7;
            rock.TextAlignment = TextAlignment.Center;
            rock.FontWeight = FontWeights.Bold;
            rock.Background = Brushes.Black;
            rock.Foreground = Brushes.SaddleBrown;
        }
        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(rock);
            Canvas.SetLeft(rock, X);
            Canvas.SetTop(rock, Y);
        }
    }

    public class IS : Baggage
    {
        TextBlock t;
        public IS(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initIS();
        }

        private void initIS()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "IS";
            t.FontSize = 12;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.White;
        }
        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class YOU : Baggage
    {
        TextBlock t;
        public YOU(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initYOU();
        }

        private void initYOU()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "YOU";
            t.FontSize = 8;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.Pink;

        }

        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class FLAG : Baggage
    {
        TextBlock t;
        public FLAG(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initFLAG();
        }

        private void initFLAG()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "F L\r\nA G";
            t.FontSize = 7;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.Yellow;
        }


        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class WIN : Baggage
    {
        TextBlock t;
        public WIN(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initWIN();
        }

        private void initWIN()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "WIN";
            t.FontSize = 8;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.Yellow;
        }

        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class PUSH : Baggage
    {
        TextBlock t;
        public PUSH(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initPUSH();
        }

        private void initPUSH()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "P U\r\nS H";
            t.FontSize = 7;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.SaddleBrown;
        }

        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class WALL : Baggage
    {
        TextBlock t;
        public WALL(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initWALL();
        }

        private void initWALL()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "W A\r\nL L";
            t.FontSize = 7;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.Gray;
        }
        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class STOP : Baggage
    {
        TextBlock t;
        public STOP(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initSTOP();
        }

        private void initSTOP()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "S T\r\nO P";
            t.FontSize = 7;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.Green;
        }
        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class SINK : Baggage
    {
        TextBlock t;
        public SINK(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initSINK();
        }

        private void initSINK()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "S I\r\nN K";
            t.FontSize = 7;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.Blue;
        }
        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }

    public class Goop : Baggage
    {
        TextBlock t;
        public Goop(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initGoop();
        }

        private void initGoop()
        {
            t = new TextBlock();
            t.Width = 20;
            t.Height = 20;
            t.Text = "G O\r\nO P";
            t.FontSize = 7;
            t.TextAlignment = TextAlignment.Center;
            t.FontWeight = FontWeights.Bold;
            t.Background = Brushes.Black;
            t.Foreground = Brushes.Blue;
        }
        public override void draw(MainWindow window)
        {
            window.canvas2d.Children.Add(t);
            Canvas.SetLeft(t, X);
            Canvas.SetTop(t, Y);
        }
    }



    public class State
    {
        private List<Wall> wallState = new List<Wall>();
        private List<Baggage> bagState = new List<Baggage>();
        private List<Area> areaState = new List<Area>();
        private List<GOOP> goopState = new List<GOOP>();
        private Player sokoState;

        public State(List<Wall> pwallState, List<Baggage> pbagState, List<Area> pareaState, List<GOOP> pgoopState, Player psokoState)
        {
            this.wallState = new List<Wall>(pwallState); // copy by value
            this.bagState = new List<Baggage>(pbagState);
            this.areaState = new List<Area>(pareaState);
            this.goopState = new List<GOOP>(pgoopState);
       
            this.sokoState = psokoState;
            
        }

        public List<Wall> getWallState { get { return wallState; } }
        public List<Baggage> getBagState { get { return bagState; } }
        public List<Area> getAreaState { get { return areaState; } }
        public List<GOOP> getGoopState { get { return goopState; } }
        public Player getSokoState { get { return sokoState; } }

    }


    public class Memento
    {
        private State state;

        public Memento(State state)
        {
            this.state = state;
        }

        public State getState()
        {
            return state;
        }
    }

    public class Originator
    {
        private State state;

        public void setState(State state)
        {
            this.state = state;
        }

        public State getState()
        {
            return state;
        }

        public Memento saveStateToMemento()
        {
            return new Memento(state);
        }

        public void getStateFromMemento(Memento memento)
        {
            state = memento.getState();
        }
    }

    public class CareTaker
    {
        private List<Memento> mementoList = new List<Memento>();

        public void add(Memento state)
        {
            mementoList.Add(state);
        }

        //public void remove(Memento state)
        //{
        //    mementoList.Remove(state);
        //}

        public void Undo()
        {
            if (mementoList.Count == 1) // At least one memento (the initial state)
                return;
            var memento = mementoList.Last();
            mementoList.Remove(memento);
        }

        public Memento getLastMemento()
        {
            return mementoList.Last();
        }
        //public int listCount()
        //{
        //    return mementoList.Count;
        //}



    }
}
