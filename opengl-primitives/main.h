

void mainDisplay(void);
void setFont(char*,int);
void drawString(GLuint,GLuint,char*);
void mainReshape(int,int);
void resetSubWindowsPosition(int,int);
void mainKeyboard(unsigned char,int,int);
void mainSpecial(int,int,int);
void reDisplayAll(void);
void topReshape(int,int);
void frontReshape(int,int);
void leftReshape(int,int);
void v3DReshape(int,int);
void mouse3d(int,int,int,int);
void motion3d(int x,int y);
void active_reshape(int,int);
void color_reshape(int,int);
void color_mouse(int,int,int,int);
void draw_color_quad(int,int,int,int);
void display(void);
void active_display(void);
void color_display(void);
void menu(int);
void reshape(int,int,double[]);

void credits_dialog(void);
void credits_mouse(int,int,int,int);
void display_credits(void);


void help_dialog(void);
void display_help(void);

void camera_normal(void);
void animate(void);

void drawAxes(void);

void drawCube(int);
void drawCuboid(int);
void drawCone(int);
void drawSphere(int);
void drawPyramid(int);
void drawTorus(int);
void drawTetrahedron(int);
void drawTeapot(int);
void (*drawModel[8])(int)={drawCube,drawCuboid,drawCone,drawSphere,
					drawPyramid,drawTorus,drawTetrahedron,drawTeapot};

///////////////MENU////////////////
void create_menu(void);
void surface_type(int);
void menu_insert(int);
void menu_size(int);
void menu_sizex(int);
void menu_sizey(int);
void menu_sizez(int);
void menu_sizeall(int);
void menu_save(int);
void menu_load(int);
void main_menu(int);

////////////////////////////////////

void object_init(void);
int create_object(int);
int surface(int);
void delete_object(void);


/////////////////////////
void copy_object(void);
void paste_object(void);



