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



namespace WpfApp6
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


        String levelText;
        int xCor;
        int yCor;
        private List<Actor> actors;
        private void buildWorld()
        {
            //the view
            actors = new List<Actor>();
            
            foreach (Wall w in walls)
                actors.Add(w);
            foreach (Baggage b in baggs)
                actors.Add(b);
            foreach (Area a in areas)
                actors.Add(a);
            foreach (GOOP g in goops)
                actors.Add(g);

            actors.Add(soko);


            levelText = "";

            for(int i = 0; OFFSET + SPACE * i <= h; i++)
            {
                yCor = OFFSET + SPACE * i;
                for (int j = 0; OFFSET + SPACE * j <= w; j++)
                {
                    xCor = OFFSET + SPACE * j;
                    if (existActor(actors, xCor, yCor))
                    {
                        levelText = levelText + getActor(actors, xCor, yCor).FirstLine;
                    }
                    else
                        levelText = levelText + "    ";
                }

                levelText = levelText + "\n";

                for (int j = 0; OFFSET + SPACE * j <= w; j++)
                {
                    xCor = OFFSET + SPACE * j;
                    if (existActor(actors, xCor, yCor))
                    {
                        levelText = levelText + getActor(actors, xCor, yCor).SecondLine;
                    }
                    else
                        levelText = levelText + "    ";
                }

                levelText = levelText + "\n";
            }

            textbox.Text = levelText;
        

        }

        private void update()
        {
            actors = new List<Actor>();

            foreach (Wall w in walls)
                actors.Add(w);
            foreach (Baggage b in baggs)
                actors.Add(b);
            foreach (Area a in areas)
                actors.Add(a);
            foreach (GOOP g in goops)
                actors.Add(g);

            actors.Add(soko);


            levelText = "";

            for (int i = 0; OFFSET + SPACE * i <= h; i++)
            {
                yCor = OFFSET + SPACE * i;
                for (int j = 0; OFFSET + SPACE * j <= w; j++)
                {
                    xCor = OFFSET + SPACE * j;
                    if (existActor(actors, xCor, yCor))
                    {
                        levelText = levelText + getActor(actors, xCor, yCor).FirstLine;
                    }
                    else
                        levelText = levelText + "    ";
                }

                levelText = levelText + "\n";

                for (int j = 0; OFFSET + SPACE * j <= w; j++)
                {
                    xCor = OFFSET + SPACE * j;
                    if (existActor(actors, xCor, yCor))
                    {
                        levelText = levelText + getActor(actors, xCor, yCor).SecondLine;
                    }
                    else
                        levelText = levelText + "    ";
                }

                levelText = levelText + "\n";
            }

            textbox.Text = levelText;
            if (iscompleted == true)
            {
                if (level_index == levels.Count - 1)
                    MessageBox.Show("You won all levels! Congrates!");
                else
                {
                    if (MessageBox.Show("You won! Click ok to proceed to the next level.") == MessageBoxResult.OK)
                        nextLevel();
                }
            }

            if (isdead == true)
            {
                MessageBox.Show("You died. Press R to restart level.");
            }



        }

        private Boolean existActor(List<Actor> actors, int xcor, int ycor)
        {
            foreach(Actor a in actors)
            {
                if (a.X == xcor && a.Y == ycor)
                    return true;
            }
            return false;
        }

        private Actor getActor(List<Actor> actors, int xcor, int ycor)
        {
            foreach (Actor a in actors)
            {
                if (a.X == xcor && a.Y == ycor)
                    return a;
            }
            return null;
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

                    foreach (Baggage bag in movableBaggs)
                    {
                        if (actor.isLeftCollision(bag))
                        {
                            foreach (Baggage item in movableBaggs)
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
            foreach (GOOP g in goops)
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
                    foreach (GOOP g in goops)
                        if (b.X == g.X && b.Y == g.Y)
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
                if ((flag.isRightCollision(i) && i.isRightCollision(you)) || (flag.isBottomCollision(i) && i.isBottomCollision(you)))
                    return true;

            return false;

        }

        public Boolean BABAIsU()
        {

            foreach (Baggage i in iss)
                if ((baba.isRightCollision(i) && i.isRightCollision(you)) || (baba.isBottomCollision(i) && i.isBottomCollision(you)))
                    return true;

            return false;

        }

        public Boolean FlagIsWin()
        {

            foreach (Baggage i in iss)
                if ((flag.isRightCollision(i) && i.isRightCollision(win)) || (flag.isBottomCollision(i) && i.isBottomCollision(win)))
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

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
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
        protected String firstLine;
        protected String secondLine;

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

        public string FirstLine
        {
            get { return firstLine; }
        }

        public string SecondLine
        {
            get { return secondLine; }
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
        
        public Wall(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initWall();
        }

        private void initWall()
        {
            this.firstLine = "##";
            this.secondLine = "##";
        }

        
    }

    public class Player : Actor
    {
        
        public Player(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            initPlayer();
        }

        private void initPlayer()
        {
            this.firstLine = "p.";
            this.secondLine = "__";

        }
        

    }

    public class Baggage : Actor  // rocks
    {
        
        public Baggage(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            initBaggage();
        }

        private void initBaggage()
        {
            this.firstLine = "oo";
            this.secondLine = "oo";
        }

        

    }

    public class GOOP : Actor 
    {
        
        public GOOP(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;

            initGOOP();
        }

        private void initGOOP()
        {
            this.firstLine = "~~";
            this.secondLine = "~~";
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
            this.firstLine = "o~";
            this.secondLine = "|_";
        }

        

    }

    // word blocks that are derived from Baggage class
    public class BABA : Baggage
    {
        
        public BABA(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initBABA();
        }

        private void initBABA()
        {
            this.firstLine = "ba";
            this.secondLine = "ba";
        }
        
    }

    public class ROCK : Baggage
    {
        
        public ROCK(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initROCK();
        }

        private void initROCK()
        {
            this.firstLine = "ro";
            this.secondLine = "ck";
        }
        
    }

    public class IS : Baggage
    {
        
        public IS(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initIS();
        }

        private void initIS()
        {
            this.firstLine = ".i..";
            this.secondLine = ".s.";
        }
        
    }

    public class YOU : Baggage
    {
        
        public YOU(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initYOU();
        }

        private void initYOU()
        {
            this.firstLine = "yo";
            this.secondLine = "u..";

        }

        
    }

    public class FLAG : Baggage
    {
        
        public FLAG(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initFLAG();
        }

        private void initFLAG()
        {
            this.firstLine = "fl";
            this.secondLine = "ag";
        }
        
    }

    public class WIN : Baggage
    {
        
        public WIN(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initWIN();
        }

        private void initWIN()
        {
            this.firstLine = "wi";
            this.secondLine = "n.";
        }
        
    }

    public class PUSH : Baggage
    {
        
        public PUSH(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initPUSH();
        }

        private void initPUSH()
        {
            this.firstLine = "pu";
            this.secondLine = "sh";
        }
        
    }

    public class WALL : Baggage
    {
        
        public WALL(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initWALL();
        }

        private void initWALL()
        {
            this.firstLine = "wa";
            this.secondLine = " l.l";
        }
        
    }

    public class STOP : Baggage
    {
        
        public STOP(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initSTOP();
        }

        private void initSTOP()
        {
            this.firstLine = "st";
            this.secondLine = "op";
        }
        
    }

    public class SINK : Baggage
    {
        
        public SINK(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initSINK();
        }

        private void initSINK()
        {
            this.firstLine = "si";
            this.secondLine = "nk";
        }
        
    }

    public class Goop : Baggage
    {
        
        public Goop(int x, int y) : base(x, y)
        {
            this.x = x;
            this.y = y;
            initGoop();
        }

        private void initGoop()
        {
            this.firstLine = "go";
            this.secondLine = "op";
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
