/*
The program implements a 3D Modeling Program using primitive 3D objects
such as cube, sphere etc. and creates a user interface to modify these 
objects. User interacts through the use of a pointing device and the 
keyboard. 3D Environments created can and saved and modified later. 
The program is interactive and user friendly 

Rahul, Samir
2000
*/
#include <GL/glut.h>
#include <stdarg.h>
#include <string.h>
#include <stdio.h>
#include <math.h>
#include "main.h"

enum{
	CUBE,CUBOID,CONE,SPHERE,PYRAMID,TORUS,TETRAHEDRON,TEAPOT
} object=CUBE;




#include "menu.h"

#define GAP				25
#define SUBWINHEIGHT	256
#define SUBWINWIDTH		256
#define NUMSUBWINDOWS	6

#define MAXOBJECTS		10
#define SHADING			0
#define WIRE			1
#define YES				1
#define NO				0


GLfloat LightAmbient[]= { 0.3f, 0.3f, 0.3f, 1.0f }; 
GLfloat LightDiffuse[]= { 0.8f, 0.8f, 0.8f, 1.0f };
GLfloat LightPosition[]= { 1.0f, -2.0f, 4.0f, 1.0f };
GLvoid *font_style=GLUT_BITMAP_TIMES_ROMAN_10;
GLuint sub_width=256;
GLuint sub_height=256;
GLuint sub1,sub2,sub3,sub4,sub5,sub6,sub7,sub8,sub9;//for sub_menus
enum{
	enTop,
	enFront,
	enLeft,
	en3D
};

GLdouble perspective[4]={60.0,1.0,1.0,10.0};
GLdouble ortho[6]={-1.2,1.2,-1.2,1.2,1.0,3.5};

GLdouble lookAt[4][9]={
	{0,0,2,			0,0,0,	   -1,0,0},
	{2,0,0,			0,0,0,		0,0,1},
	{0,-2,0,		0,0,0,		0,0,1},
	{2,0.5,0.5,		0,0,0,		0,0,1}
};

GLdouble oldlook[4][9]={
	{0,0,2,			0,0,0,	   -1,0,0},
	{2,0,0,			0,0,0,		0,0,1},
	{0,-2,0,		0,0,0,		0,0,1},
	{2,0.5,0.5,		0,0,0,		0,0,1}
};

struct Window{
	char* title;
	GLuint hWnd;
	int left;
	int top;
	void (*reshape)(int width,int height);
	void (*display)(void);
	void (*menu)(int value);
	void (*keyboard)(unsigned char key,int x,int y);
	void (*mouse)(int button,int state,int x,int y);
	void (*motion)(int x,int y);
} windowDef[]={
	{
		"Front",
		0,
		GAP*2+SUBWINWIDTH,
		GAP,
		frontReshape,
		display,
		0,
		mainKeyboard,
		0,
		0
	},

	{
		"Left",
		0,
		GAP,
		GAP*2+SUBWINHEIGHT,
		leftReshape,
		display,
		0,
		mainKeyboard,
		0,
		0
	},

	{
		"Top",
		0,
		GAP,
		GAP,
		topReshape,
		display,
		0,
		mainKeyboard,
		0,
		0
	},

	{
		"3D View",
		0,
		GAP*2+SUBWINWIDTH,
		GAP*2+SUBWINHEIGHT,
		v3DReshape,
		display,
		menu,
		mainKeyboard,
		mouse3d,
		motion3d
	},
	{
		"Active",
		0,
		GAP*3+2*SUBWINWIDTH,
		GAP,
		active_reshape,
		active_display,
		0,
		mainKeyboard,
		0,
		0
	},
	{
		"Color",
		0,
		GAP*3+2*SUBWINWIDTH,
		GAP*2+SUBWINHEIGHT,
		color_reshape,
		color_display,
		0,
		mainKeyboard,
		color_mouse,
		0
	}
};

enum{
	PERSPECTIVE,ORTHO
} mode=ORTHO;


typedef struct{
	int obj;
	int present;
	int shad_or_wire;
	int surfaces;
	float surf_color[6][4];
	float angle[3];
	float position[3];
	float scale[3];
	void (*draw)(int);
}Objects;

int Active_Object=-1;
float Zoom=0.0;
int Translate_Flag=0;
int Rotate_Flag=0;
int Num_Objects=0;
int xmotion=0;
int ymotion=0;
int Animate_Flag=0;
int Animate_ctr=0;
int Animate_Key=0;
int Animate_Prev=0;
FILE *file;
int CL_GAP=10;

Objects Copy;
Objects Object[MAXOBJECTS];
 float temp[6][4]={
		{1.0,0.0,0.0,0.0},
		{1.0,1.0,0.0,0.0},
		{1.0,0.0,1.0,0.0},
		{0.0,1.0,0.0,0.0},
		{0.0,0.0,1.0,0.0},
		{0.0,1.0,1.0,0.0}
	};


int main(int argc,char** argv){
	
	int i;
	GLuint window;
	

	glutInit(&argc,argv);
	glutInitDisplayMode(GLUT_DEPTH|GLUT_RGB|GLUT_DOUBLE);
	glutInitWindowSize(4*GAP + 3*SUBWINWIDTH, 3*GAP + 2*SUBWINHEIGHT);
	glutInitWindowPosition(50,50);

	window=glutCreateWindow("CS3241 Assignment 1");
	glutDisplayFunc(mainDisplay);
	glutReshapeFunc(mainReshape);
	glutKeyboardFunc(mainKeyboard);
	glutSpecialFunc(mainSpecial);
	glutIdleFunc(animate);
	create_menu();



	for(i=0;i<NUMSUBWINDOWS;i++){

		windowDef[i].hWnd=glutCreateSubWindow(window,windowDef[i].left,
			windowDef[i].top,SUBWINWIDTH,SUBWINHEIGHT);
		if(windowDef[i].reshape)
			glutReshapeFunc(windowDef[i].reshape);
		if(windowDef[i].display)
			glutDisplayFunc(windowDef[i].display);
		if(windowDef[i].keyboard)
			glutKeyboardFunc(windowDef[i].keyboard);
		if(windowDef[i].mouse)
			glutMouseFunc(windowDef[i].mouse);
		if(windowDef[i].motion)
			glutMotionFunc(windowDef[i].motion);
		
		if(i==5)
			glutSetCursor(GLUT_CURSOR_CROSSHAIR);
	
		create_menu();

	}

	object_init();




	reDisplayAll();
		
	glutMainLoop();
	return 0;
}


void mainDisplay(void){
	int i;

	glClearColor(0.8,0.8,0.8,0.0);
	glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);
	glColor3ub(0,0,0);
	setFont("helvetica",12);
	


	for(i=0;i<NUMSUBWINDOWS;i++){
		drawString(windowDef[i].left,windowDef[i].top-5,windowDef[i].title);
	}

	glutSwapBuffers();
}

void setFont(char* name,int size){

	font_style=GLUT_BITMAP_HELVETICA_10;
	
	if(strcmp(name,"helvetica")==0){
		if(size==12)
			font_style=GLUT_BITMAP_HELVETICA_12;
		else if (size==18)
			font_style=GLUT_BITMAP_HELVETICA_18;
	} else if (strcmp(name,"times roman")==0){
		font_style=GLUT_BITMAP_TIMES_ROMAN_10;
		if(size==24)
			font_style=GLUT_BITMAP_TIMES_ROMAN_24;
	} else if (strcmp(name,"8x13")==0){
		font_style=GLUT_BITMAP_8_BY_13;
	} else if (strcmp(name,"9x15")==0){
		font_style=GLUT_BITMAP_9_BY_15;
	}
}

void drawString(GLuint x,GLuint y,char* format){

	char buffer[255],*s;
	
	sprintf(buffer,"%s",format);
	glRasterPos2i(x,y);
	for(s=buffer; *s; s++)
		glutBitmapCharacter(font_style,*s);

}

void mainReshape(int width,int height){

	int i;

	glViewport(0,0,width,height);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluOrtho2D(0,width,height,0);
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	sub_width=(width-4*GAP)/3.0;
	sub_height=(height-3*GAP)/2.0;

	resetSubWindowsPosition(sub_width,sub_height);
	for(i=0;i<NUMSUBWINDOWS;i++){
		glutSetWindow(windowDef[i].hWnd);
		glutPositionWindow(windowDef[i].left,windowDef[i].top);
		glutReshapeWindow(sub_width,sub_height);
	}

}

void resetSubWindowsPosition(int width,int height){
	
	int i;

	for(i=0;i<NUMSUBWINDOWS-2;i++){
		windowDef[i].left=GAP+(i%2?GAP+width:0);
		windowDef[i].top=GAP+(i/2?GAP+height:0);
	}
	windowDef[4].left=3*GAP+2*width;
	windowDef[4].top=GAP;

	windowDef[5].left=3*GAP+2*width;
	windowDef[5].top=2*GAP+height;
}

void mainSpecial(int key,int x,int y){
	
	int t=0,alpha=0;
	GLdouble zz,xx,yy,xdash=0.0,zdash=0.0;
	xx=lookAt[en3D][0];
	zz=lookAt[en3D][2];
	yy=lookAt[en3D][1];


	switch(key){

	case GLUT_KEY_F1:
		help_dialog();
		break;
	case GLUT_KEY_F2:
		credits_dialog();
		break;
	case GLUT_KEY_F3:
		Animate_Flag=1;

		break;
	case GLUT_KEY_F4:
		Animate_Flag=0;
		break;
	case GLUT_KEY_UP:

		if(xx>=0){
			if(zz>=0){
				xx-=0.10307764;
				zz=sqrt(4.25-xx*xx);
				lookAt[en3D][6]-=0.10307764;
				lookAt[en3D][8]=sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
			}else{
				xx+=0.10307764;
				zz=-sqrt(4.25-xx*xx);
				lookAt[en3D][6]-=0.10307764;
				lookAt[en3D][8]=sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
			}
		}else if(zz>=0){
			xx-=0.10307764;
			zz=sqrt(4.25-xx*xx);
			lookAt[en3D][6]+=0.10307764;
			lookAt[en3D][8]=-sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
		}else{
			xx+=0.10307664;
			zz=-sqrt(4.25-xx*xx);
			lookAt[en3D][6]+=0.10307764;
			lookAt[en3D][8]=-sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
		}

		lookAt[en3D][0]=xx;
		lookAt[en3D][2]=zz;
		

		if(xx>=1.9){
			zz=0.5;
			lookAt[en3D][0]=2.0;
			lookAt[en3D][2]=0.5;
			lookAt[en3D][6]=0.0;
			lookAt[en3D][8]=1.0;
		}

		break;
	case GLUT_KEY_DOWN:

		
		if(xx>=2.0){
			zz=-0.01;
			xx=2.0;
		}
		if(xx>=0){
			if(zz>=0){
				xx+=0.10307764;
				zz=sqrt(4.25-xx*xx);
				lookAt[en3D][6]+=0.10307764;
				lookAt[en3D][8]=sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
			}else{
				xx-=0.10307764;
				zz=-sqrt(4.25-xx*xx);
				lookAt[en3D][6]+=0.10307764;
				lookAt[en3D][8]=sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
			}
		}else if(zz>=0){
			xx+=0.10307764;
			zz=sqrt(4.25-xx*xx);
			lookAt[en3D][6]-=0.10307764;
			lookAt[en3D][8]=-sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
		}else{
			xx-=0.10307664;
			zz=-sqrt(4.25-xx*xx);
			lookAt[en3D][6]-=0.10307764;
			lookAt[en3D][8]=-sqrt(4.25-lookAt[en3D][6]*lookAt[en3D][6]);
		}

		lookAt[en3D][0]=xx;
		lookAt[en3D][2]=zz;

		

		if(xx<=-1.9){
			zz=0.5;
			lookAt[en3D][0]=-1.8;
			lookAt[en3D][2]=0.1;
			lookAt[en3D][6]=0.0;
			lookAt[en3D][8]=-1.0;
		}

		
		break;
	case GLUT_KEY_LEFT:
	
		
		if(xx>=2.0){
			yy=-0.01;
			xx=2.0;
		}
		if(xx>=0){
			if(yy>=0){
				xx+=0.10307764;
				yy=sqrt(4.25-xx*xx);
			}else{
				xx-=0.10307764;
				yy=-sqrt(4.25-xx*xx);
			}
		}else if(yy>=0){
			xx+=0.10307764;
			yy=sqrt(4.25-xx*xx);
		}else{
			xx-=0.10307664;
			yy=-sqrt(4.25-xx*xx);

		}

		lookAt[en3D][0]=xx;
		lookAt[en3D][1]=yy;


		

		if(xx<=-1.9){
			
			lookAt[en3D][0]=-1.9;
			lookAt[en3D][1]=0.01;	
		}

		
		break;
	case GLUT_KEY_RIGHT:
		if(xx>=0){
			if(yy>=0){
				xx-=0.10307764;
				yy=sqrt(4.25-xx*xx);
			}else{
				xx+=0.10307764;
				yy=-sqrt(4.25-xx*xx);
			}
		}else if(yy>=0){
			xx-=0.10307764;
			yy=sqrt(4.25-xx*xx);
		}else{
			xx+=0.10307664;
			yy=-sqrt(4.25-xx*xx);

		}

		lookAt[en3D][0]=xx;
		lookAt[en3D][1]=yy;


		

		if(xx>=1.9){
			
			lookAt[en3D][0]=2.0;
			lookAt[en3D][1]=0.5;	
		}




	
		break;
	


	}
	reDisplayAll();

  
}

void camera_normal(){
	
	int i,j;
	for(i=0;i<4;i++)
		for(j=0;j<9;j++)
			lookAt[i][j]=oldlook[i][j];


}

void mainKeyboard(unsigned char key,int x,int y){

	int i=0;

	switch(key){

	case 'p':
		mode=PERSPECTIVE;
		break;
	case 'o':
		mode=ORTHO;
		break;
	case '0':
		create_object(CUBE);
		break;
	case '1':
		create_object(CUBOID);
		break;
	case '2':
		create_object(CONE);
		break;
	case '3':
		create_object(SPHERE);
		break;
	case '4':
		create_object(PYRAMID);
		break;
	case '5':
		create_object(TORUS);
		break;
	case '6':
		create_object(TETRAHEDRON);
		break;
	case '7':
		create_object(TEAPOT);
		break;
	case 'f':
		glutFullScreen();
		break;
	case ' ':
		glutReshapeWindow(SUBWINWIDTH*3+GAP*4,SUBWINHEIGHT*2+GAP*3);
		break;
	case 'n':
		glutReshapeWindow(SUBWINWIDTH*3+GAP*4,SUBWINHEIGHT*2+GAP*3);
		break;
	case '+':
		if(mode==PERSPECTIVE)
			Zoom+=0.2;
		break;
	case '-':
		if(mode==PERSPECTIVE)
			Zoom-=0.2;
		break;
	case '=':
		if(mode==PERSPECTIVE)
			Zoom=0.0;
		break;
	case 'a':
		if(mode==ORTHO && Active_Object!=-1){
			Object[Active_Object].position[0]+=0.1;
		}
		break;
	case 'z':
		if(mode==ORTHO && Active_Object!=-1){
			Object[Active_Object].position[0]-=0.1;
		}
		break;
	case 'x':
		if(Active_Object!=-1){
			Object[Active_Object].position[0]=0.0;
			Object[Active_Object].position[1]=0.0;
			Object[Active_Object].position[2]=0.0;
		}
		break;
	case 'e':
		if(mode==ORTHO && Active_Object!=-1){
			Object[Active_Object].angle[0]+=4.0;
		}
		break;
	case 'd':
		if(mode==ORTHO && Active_Object!=-1){
			Object[Active_Object].angle[0]-=4.0;
		}
		break;
	case 'c':
		if(mode==ORTHO && Active_Object!=-1){
			Object[Active_Object].angle[0]=0.0;
			Object[Active_Object].angle[1]=0.0;
			Object[Active_Object].angle[2]=0.0;
		}
		break;
	case 13:
		camera_normal();
		break;
	case 9:
		if(Num_Objects!=0){
			if(Active_Object==MAXOBJECTS-1)
				i=0;
			else
				i=Active_Object+1;
			while(Object[i].present==NO){
				i++;
				if(i==MAXOBJECTS)
					i=0;
			}
			Active_Object=i;
		}
		break;
	
	case 27:
		exit(0);
	}

	reDisplayAll();

}

void reDisplayAll(void){
	int i;

	for(i=0;i<NUMSUBWINDOWS;i++){
		glutSetWindow(windowDef[i].hWnd);
		
		windowDef[i].reshape(sub_width,sub_height);
	////////////////////
		glLightfv(GL_LIGHT1, GL_AMBIENT, LightAmbient);
		glLightfv(GL_LIGHT1, GL_DIFFUSE, LightDiffuse);	
		glLightfv(GL_LIGHT1, GL_POSITION,LightPosition);
		glEnable(GL_LIGHT1);
	////////////////////

		glutPostRedisplay();
	}

}

void topReshape(int width,int height){

	reshape(width,height,lookAt[enTop]);

}

void frontReshape(int width,int height){

	reshape(width,height,lookAt[enFront]);

}

void leftReshape(int width,int height){

	reshape(width,height,lookAt[enLeft]);


}

void v3DReshape(int width,int height){

	reshape(width,height,lookAt[en3D]);


}

void mouse3d(int button,int state,int x,int y){
	
	xmotion=x;
	ymotion=y;
	y=sub_height-y;
	if(Active_Object!=-1 && button==GLUT_LEFT_BUTTON 
		&& mode==ORTHO && state==GLUT_DOWN && Translate_Flag==1){
		
		Translate_Flag=0;

		
		Object[Active_Object].position[1]=(float)(((float)x*2.4)/sub_width-1.2);


		Object[Active_Object].position[2]=(float)(((float)y*2.4)/sub_height-1.2);
	}
	reDisplayAll();
	

}

void motion3d(int x,int y){
	int xdash,ydash;

	xdash=x-xmotion;
	ydash=y-ymotion;
	xmotion=x;
	ymotion=y;
	
	if(mode==ORTHO && Active_Object!=-1 && Rotate_Flag==1){

		
		if(xdash>0)
			Object[Active_Object].angle[2]+=4.0;
		else
			Object[Active_Object].angle[2]-=4.0;
		if(ydash>0)
			Object[Active_Object].angle[1]+=4.0;
		else
			Object[Active_Object].angle[1]-=4.0;
	}

	reDisplayAll();
}

void active_reshape(int width,int height){

	glViewport(0,0,width,height);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	glOrtho(ortho[0],ortho[1],ortho[2],ortho[3],ortho[4],
									ortho[5]);
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();
	gluLookAt(2,0.5,0.5,0,0,0,0,0,1);
	glClearColor(0.2,0.2,0.2,0.0);
	glEnable(GL_DEPTH_TEST);
	glEnable(GL_LIGHTING);
	glEnable(GL_COLOR_MATERIAL);

}

void color_reshape(int width,int height){
	
	glViewport(0,0,width,height);
	glEnable(GL_DEPTH_TEST);
	glEnable(GL_COLOR_MATERIAL);
}

void color_mouse(int button,int state,int x,int y){

	//get surface of click
	
	int winWidth;
	int winHeight;
	int surf;
	int a,b;
	float xp,yp;
	float clR,clG,clB,clA;
	surf=0;
	a=0;b=0;
	
	y=sub_height-y;
	winWidth=(int)(sub_width-2*CL_GAP)/3;
	winHeight=(int)(sub_height-5*CL_GAP)/2;

	if(x<winWidth){
		if (y<winHeight){
			surf=4;
		}
		else{
			if(y>winHeight+3*CL_GAP && y<2*winHeight+3*CL_GAP){
				surf=1;
			}
		}
	}
	else{
		if(x>winWidth+CL_GAP && x<2*winWidth+CL_GAP){
			if(y<winHeight){
				surf=5;
			}
			else {
				if(y>winHeight+3*CL_GAP && y<2*winHeight+3*CL_GAP){
					surf=2;
				}
			}
		}
		else{
			if(x>2*winWidth+2*CL_GAP){
				if(y<winHeight){
					surf=6;
				}
				else {
					if(y>winHeight+3*CL_GAP && y<2*winHeight+3*CL_GAP){
						surf=3;
					}
				}
			}
		}
	}

	if(surf!=0){
		switch(surf){
		case 1:
			a=0;
			b=winHeight+3*CL_GAP;
			break;
		case 2:
			a=winWidth+CL_GAP;
			b=winHeight+3*CL_GAP;
			break;
		case 3:
			a=2*winWidth+2*CL_GAP;
			b=winHeight+3*CL_GAP;
			break;
		case 4:
			a=0;
			b=0;
			break;
		case 5:
			a=winWidth+CL_GAP;
			b=0;
			break;
		case 6:
			a=2*winWidth+2*CL_GAP;
			b=0;
			break;
		}
		xp=((float)(x-a))/winWidth;
		yp=((float)(y-b))/winHeight;

		clR=1.0-sqrt((xp*xp+yp*yp)/2);
		clG=1.0-sqrt(((1-yp)*(1-yp)+xp*xp)/2);
		clB=1.0-sqrt((yp*yp+(1-xp)*(1-xp))/2);
		clA=1.0-sqrt(((1-yp)*(1-yp)+(1-xp)*(1-xp))/2);


	
		
		surf--;
		if(Active_Object != -1){
			Object[Active_Object].surf_color[surf][0]=clR;
			Object[Active_Object].surf_color[surf][1]=clG;
			Object[Active_Object].surf_color[surf][2]=clB;
			Object[Active_Object].surf_color[surf][3]=clA;

			reDisplayAll();
		}
	}



}

void draw_color_quad(int x, int y, int w, int h){
	glBegin(GL_QUADS);

		glColor4f(1.0,0.0,0.0,0.0);
		glVertex2f(x,y);

		glColor4f(0.0,1.0,0.0,0.0);
		glVertex2f(x,y+h);

		glColor4f(0.0,0.0,0.0,1.0);
		glVertex2f(x+w,y+h);

		glColor4f(0.0,0.0,1.0,0.0);
		glVertex2f(x+w,y);

	glEnd();
}




void display(void){

	int i;

	glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);
	
	
	for(i=0;i<MAXOBJECTS;i++){
		if(Object[i].present==YES)
			Object[i].draw(i);
	}


	glutSwapBuffers();
}

void active_display(void){
	
	glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);
	if(Active_Object!=-1)
		Object[Active_Object].draw(Active_Object);

	
	glutSwapBuffers();


}

void color_display(){

	int winWidth;
	int winHeight;

	winWidth=(sub_width-2*CL_GAP)/3;
	winHeight=(sub_height-5*CL_GAP)/2;

	glClearColor(0.8,0.8,0.8,1.0);
	glClear(GL_COLOR_BUFFER_BIT|GL_DEPTH_BUFFER_BIT);

	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluOrtho2D(0.0, sub_width, 0.0,sub_height);
	glMatrixMode(GL_MODELVIEW);
	//draw six color quads
	
	draw_color_quad(0,0,winWidth,winHeight);

	draw_color_quad(winWidth+CL_GAP,0,
		winWidth,winHeight);

	draw_color_quad(2*winWidth+2*CL_GAP,0,winWidth,winHeight);

	draw_color_quad(0,(sub_height-3*CL_GAP)/2+CL_GAP,winWidth,winHeight);

	draw_color_quad((sub_width-2*CL_GAP)/3+CL_GAP,
		(sub_height-3*CL_GAP)/2+CL_GAP,winWidth,winHeight);

	draw_color_quad(2*(sub_width-2*CL_GAP)/3+2*CL_GAP,
		(sub_height-3*CL_GAP)/2+CL_GAP,winWidth,winHeight);

	setFont("helvitica",12);
	glColor3f(0.0,0.0,0.0);
	// titles of quads
	drawString(0,2*winHeight+2*CL_GAP+5,"Surface 1");
	drawString(winWidth+CL_GAP,2*winHeight+2*CL_GAP+5,"Surface 2");
	drawString(2*winWidth+2*CL_GAP,2*winHeight+2*CL_GAP+5,"Surface 3");
	drawString(0,winHeight+5,"Surface 4");
	drawString(winWidth+CL_GAP,winHeight+5,"Surface 5");
	drawString(2*winWidth+2*CL_GAP,winHeight+5,"Surface 6");

	glutSwapBuffers();
}




void menu(int value){

	

	reDisplayAll();
}






void reshape(int width,int height,double lookat[]){
	
	glViewport(0,0,width,height);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	if (mode==PERSPECTIVE)
		gluPerspective(perspective[0]-Zoom,perspective[1],perspective[2],
							perspective[3]);
	else if (mode==ORTHO)
		glOrtho(ortho[0],ortho[1],ortho[2],ortho[3],
							ortho[4],ortho[5]);

	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();
	gluLookAt(lookat[0],lookat[1],lookat[2],
		lookat[3],lookat[4],lookat[5],lookat[6],lookat[7],lookat[8]);

	glClearColor(0.2,0.2,0.2,0.0);



	glEnable(GL_DEPTH_TEST);
	glEnable(GL_LIGHTING);

	glEnable(GL_COLOR_MATERIAL);
}

void drawAxes(){
	
	glColor3ub(255, 255, 255);
    glBegin(GL_LINE_STRIP);
    glVertex3f(0.0, 0.0, 0.0);
    glVertex3f(1.0, 0.0, 0.0);
    glVertex3f(0.75, 0.25, 0.0);
    glVertex3f(0.75, -0.25, 0.0);
    glVertex3f(1.0, 0.0, 0.0);
    glVertex3f(0.75, 0.0, 0.25);
    glVertex3f(0.75, 0.0, -0.25);
    glVertex3f(1.0, 0.0, 0.0);
    glEnd();
    glBegin(GL_LINE_STRIP);
    glVertex3f(0.0, 0.0, 0.0);
    glVertex3f(0.0, 1.0, 0.0);
    glVertex3f(0.0, 0.75, 0.25);
    glVertex3f(0.0, 0.75, -0.25);
    glVertex3f(0.0, 1.0, 0.0);
    glVertex3f(0.25, 0.75, 0.0);
    glVertex3f(-0.25, 0.75, 0.0);
    glVertex3f(0.0, 1.0, 0.0);
    glEnd();
    glBegin(GL_LINE_STRIP);
    glVertex3f(0.0, 0.0, 0.0);
    glVertex3f(0.0, 0.0, 1.0);
    glVertex3f(0.25, 0.0, 0.75);
    glVertex3f(-0.25, 0.0, 0.75);
    glVertex3f(0.0, 0.0, 1.0);
    glVertex3f(0.0, 0.25, 0.75);
    glVertex3f(0.0, -0.25, 0.75);
    glVertex3f(0.0, 0.0, 1.0);
    glEnd();
    
    glColor3ub(255, 255, 255);
    glRasterPos3f(1.1, 0.0, 0.0);
    glutBitmapCharacter(GLUT_BITMAP_HELVETICA_12, 'x');
    glRasterPos3f(0.0, 1.1, 0.0);
    glutBitmapCharacter(GLUT_BITMAP_HELVETICA_12, 'y');
    glRasterPos3f(0.0, 0.0, 1.1);
    glutBitmapCharacter(GLUT_BITMAP_HELVETICA_12, 'z');
}







void drawCube(int i){

	drawAxes();

	glPushMatrix();
	
	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	if(Object[i].shad_or_wire==WIRE){
		glBegin(GL_LINE_LOOP);
			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
						Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
		
			glVertex3f(0.3,0.3,0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(-0.3,0.3,0.3);

			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
						Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
		
			glVertex3f(0.3,-0.3,0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,0.3);

			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
						Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
	
			glVertex3f(0.3,0.3,0.3);
			glVertex3f(-0.3,0.3,0.3);
			glVertex3f(-0.3,-0.3,0.3);
			glVertex3f(0.3,-0.3,0.3);

			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
						Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
	
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
	
			glColor4f(Object[i].surf_color[4][0],Object[i].surf_color[4][1],Object[i].surf_color[4][2],Object[i].surf_color[4][3]);

			glVertex3f(0.3,0.3,0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(0.3,-0.3,0.3);

			glColor4f(Object[i].surf_color[5][0],Object[i].surf_color[5][1],Object[i].surf_color[5][2],Object[i].surf_color[5][3]);

			glVertex3f(-0.3,0.3,0.3);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,0.3);
	
		glEnd();
	}
	else{
	
		glBegin(GL_QUADS);
				
			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
						Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
		
			glVertex3f(0.3,0.3,0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(-0.3,0.3,0.3);

			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
						Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
		
			glVertex3f(0.3,-0.3,0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,0.3);

			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
						Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
	
			glVertex3f(0.3,0.3,0.3);
			glVertex3f(-0.3,0.3,0.3);
			glVertex3f(-0.3,-0.3,0.3);
			glVertex3f(0.3,-0.3,0.3);

			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
						Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
	
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
	
			glColor4f(Object[i].surf_color[4][0],Object[i].surf_color[4][1],
						Object[i].surf_color[4][2],Object[i].surf_color[4][3]);

			glVertex3f(0.3,0.3,0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(0.3,-0.3,0.3);

			glColor4f(Object[i].surf_color[5][0],Object[i].surf_color[5][1],
						Object[i].surf_color[5][2],Object[i].surf_color[5][3]);

			glVertex3f(-0.3,0.3,0.3);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,0.3);
	
		glEnd();
	}
	glPopMatrix();
}

void drawCuboid(int i){
	drawAxes();
	
	glPushMatrix();

	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	if(Object[i].shad_or_wire==WIRE){
		glBegin(GL_LINE_LOOP);

			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
					Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
		
			glVertex3f(0.5,0.3,0.3);
			glVertex3f(0.5,0.3,-0.3);
			glVertex3f(-0.5,0.3,-0.3);
			glVertex3f(-0.5,0.3,0.3);
		
			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
					Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
			glVertex3f(0.5,-0.3,0.3);
			glVertex3f(0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,0.3);

			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
					Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
	
			glVertex3f(0.5,0.3,0.3);
			glVertex3f(-0.5,0.3,0.3);
			glVertex3f(-0.5,-0.3,0.3);
			glVertex3f(0.5,-0.3,0.3);

			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
					Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
	
			glVertex3f(-0.5,0.3,-0.3);
			glVertex3f(0.5,0.3,-0.3);
			glVertex3f(0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,-0.3);
	
			glColor4f(Object[i].surf_color[4][0],Object[i].surf_color[4][1],
					Object[i].surf_color[4][2],Object[i].surf_color[4][3]);

			glVertex3f(0.5,0.3,0.3);
			glVertex3f(0.5,0.3,-0.3);
			glVertex3f(0.5,-0.3,-0.3);
			glVertex3f(0.5,-0.3,0.3);
		
			glColor4f(Object[i].surf_color[5][0],Object[i].surf_color[5][1],
					Object[i].surf_color[5][2],Object[i].surf_color[5][3]);
			
			glVertex3f(-0.5,0.3,0.3);
			glVertex3f(-0.5,0.3,-0.3);
			glVertex3f(-0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,0.3);
	
		glEnd();
	}
	else{
	
		glBegin(GL_QUADS);
			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
					Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
		
			glVertex3f(0.5,0.3,0.3);
			glVertex3f(0.5,0.3,-0.3);
			glVertex3f(-0.5,0.3,-0.3);
			glVertex3f(-0.5,0.3,0.3);
		
			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
					Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
			glVertex3f(0.5,-0.3,0.3);
			glVertex3f(0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,0.3);

			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
					Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
	
			glVertex3f(0.5,0.3,0.3);
			glVertex3f(-0.5,0.3,0.3);
			glVertex3f(-0.5,-0.3,0.3);
			glVertex3f(0.5,-0.3,0.3);

			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
					Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
	
			glVertex3f(-0.5,0.3,-0.3);
			glVertex3f(0.5,0.3,-0.3);
			glVertex3f(0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,-0.3);
	
			glColor4f(Object[i].surf_color[4][0],Object[i].surf_color[4][1],
					Object[i].surf_color[4][2],Object[i].surf_color[4][3]);

			glVertex3f(0.5,0.3,0.3);
			glVertex3f(0.5,0.3,-0.3);
			glVertex3f(0.5,-0.3,-0.3);
			glVertex3f(0.5,-0.3,0.3);
		
			glColor4f(Object[i].surf_color[5][0],Object[i].surf_color[5][1],
					Object[i].surf_color[5][2],Object[i].surf_color[5][3]);
			
			glVertex3f(-0.5,0.3,0.3);
			glVertex3f(-0.5,0.3,-0.3);
			glVertex3f(-0.5,-0.3,-0.3);
			glVertex3f(-0.5,-0.3,0.3);

		glEnd();
	}

	glPopMatrix();

}

void drawCone(int i){
	drawAxes();

	glPushMatrix();

	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	if(Object[i].shad_or_wire==WIRE){
		glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
				Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
		glutWireCone(0.5,1.0,20,20);
	}
	else{
		glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
				Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
		glutWireCone(0.5,1.0,220,220);
	}
	
	glPopMatrix();
}

void drawSphere(int i){
	drawAxes();

	glPushMatrix();

	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
				Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
	if(Object[i].shad_or_wire==WIRE){	
		glutWireSphere(0.5,20,20);
	}
	else{
		glutSolidSphere(0.5,800,800);
	}

	glPopMatrix();
}

void drawPyramid(int i){
	drawAxes();

	glPushMatrix();

	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	if(Object[i].shad_or_wire==WIRE){
		glBegin(GL_LINE_LOOP);
			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
						Object[i].surf_color[0][2],Object[i].surf_color[0][3]);

			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,0.3,-0.3);

		glEnd();

		glBegin(GL_LINE_LOOP);
	
			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
						Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);
		
			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
						Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);
				
			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
						Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);

			glColor4f(Object[i].surf_color[4][0],Object[i].surf_color[4][1],
						Object[i].surf_color[4][2],Object[i].surf_color[4][3]);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);

		glEnd();
	}
	else{
		glBegin(GL_QUADS);
			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
						Object[i].surf_color[0][2],Object[i].surf_color[0][3]);

			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,0.3,-0.3);

		glEnd();

		glBegin(GL_TRIANGLES);
	
			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
						Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);
		
			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
						Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
			glVertex3f(0.3,-0.3,-0.3);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);
				
			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
						Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
			glVertex3f(-0.3,-0.3,-0.3);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);

			glColor4f(Object[i].surf_color[4][0],Object[i].surf_color[4][1],
						Object[i].surf_color[4][2],Object[i].surf_color[4][3]);
			glVertex3f(-0.3,0.3,-0.3);
			glVertex3f(0.3,0.3,-0.3);
			glVertex3f(0.0,0.0,0.3);

		glEnd();
	}

	glPopMatrix();
}

void drawTorus(int i){
	drawAxes();

	glPushMatrix();

	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
					Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
	if(Object[i].shad_or_wire==WIRE){
		
		glutWireTorus(0.4,0.6,20,20);
	}
	else{
		
		glutSolidTorus(0.4,0.6,300,300);
	}

	glPopMatrix();

}



void drawTetrahedron(int i){
	drawAxes();

	glPushMatrix();

	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	if(Object[i].shad_or_wire==WIRE){
	
		glBegin(GL_LINE_LOOP);

			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
					Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
			glVertex3f(0.0,0.7,-0.5);
			glVertex3f(0.61,-0.3,-0.5);
			glVertex3f(-0.61,-0.3,-0.5);
		
			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
						Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
			glVertex3f(0.0,0.7,-0.5);
			glVertex3f(0.61,-0.3,-0.5);
			glVertex3f(0.0,0.0,0.5);
		
		
			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
						Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
			glVertex3f(0.61,-0.3,-0.5);
			glVertex3f(-0.61,-0.3,-0.5);
			glVertex3f(0.0,0.0,0.5);

		
			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
						Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
			glVertex3f(-0.61,-0.3,-0.5);
			glVertex3f(0.0,0.7,-0.5);
			glVertex3f(0.0,0.0,0.5);

		glEnd();
	}
	else{
			glBegin(GL_TRIANGLES);

			glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
						Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
			glVertex3f(0.0,0.7,-0.5);
			glVertex3f(0.61,-0.3,-0.5);
			glVertex3f(-0.61,-0.3,-0.5);
		
			glColor4f(Object[i].surf_color[1][0],Object[i].surf_color[1][1],
						Object[i].surf_color[1][2],Object[i].surf_color[1][3]);
			glVertex3f(0.0,0.7,-0.5);
			glVertex3f(0.61,-0.3,-0.5);
			glVertex3f(0.0,0.0,0.5);
		
		
			glColor4f(Object[i].surf_color[2][0],Object[i].surf_color[2][1],
						Object[i].surf_color[2][2],Object[i].surf_color[2][3]);
			glVertex3f(0.61,-0.3,-0.5);
			glVertex3f(-0.61,-0.3,-0.5);
			glVertex3f(0.0,0.0,0.5);

		
			glColor4f(Object[i].surf_color[3][0],Object[i].surf_color[3][1],
						Object[i].surf_color[3][2],Object[i].surf_color[3][3]);
			glVertex3f(-0.61,-0.3,-0.5);
			glVertex3f(0.0,0.7,-0.5);
			glVertex3f(0.0,0.0,0.5);

		glEnd();
	}


	glPopMatrix();

}

void drawTeapot(int i){
	drawAxes();

	glPushMatrix();

	glTranslatef(Object[i].position[0],Object[i].position[1],Object[i].position[2]);
	glRotatef(Object[i].angle[0],1.0,0.0,0.0);
	glRotatef(Object[i].angle[1],0.0,1.0,0.0);
	glRotatef(Object[i].angle[2],0.0,0.0,1.0);
	glScalef(Object[i].scale[0],Object[i].scale[1],Object[i].scale[2]);
	glColor4f(Object[i].surf_color[0][0],Object[i].surf_color[0][1],
					Object[i].surf_color[0][2],Object[i].surf_color[0][3]);
	if(Object[i].shad_or_wire==WIRE){
	
		glutWireTeapot(0.3);
	}
	else{
		
		glutSolidTeapot(0.3);
	}

	glPopMatrix();


}


void menu_insert(int x){
	//check for insert menu event

	switch(x){
	case MENU_CUBE:
		create_object(CUBE);
		break;
	case MENU_CUBOID:
		create_object(CUBOID);
		break;
	case MENU_PYRAMID:
		create_object(PYRAMID);
		break;
	case MENU_SPHERE:
		create_object(SPHERE);
		break;
	case MENU_CONE:
		create_object(CONE);
		break;
	case MENU_TETR:
		create_object(TETRAHEDRON);
		break;
	case MENU_TORUS:
		create_object(TORUS);
		break;
	case MENU_TEA:
		create_object(TEAPOT);
		break;
	}

	reDisplayAll();
}


void main_menu(int x){
	//top level menu

	switch(x){
	case MENU_QUIT:
		exit(0);
		break;
	case MENU_DELETE:
		delete_object();
		break;
	case MENU_ZOOM_IN:
		if(mode==PERSPECTIVE)
			Zoom+=0.2;
		break;
	case MENU_ZOOM_OUT:
		if (mode==PERSPECTIVE)
			Zoom-=0.2;
		break;
	case MENU_TRANSLATE:
		if(Active_Object!=-1 && mode==ORTHO)
			Translate_Flag=1;
		break;
	case MENU_ROTATE:
		if(Active_Object!=-1 && mode==ORTHO)
			Rotate_Flag=1;
		break;
	case MENU_ROTATE_OFF:
		Rotate_Flag=0;
		break;
	case MENU_CREDITS:
		credits_dialog();
		break;
	case MENU_HELP:
		help_dialog();
		break;
	case MENU_ANIMATE:
		Animate_Flag=1;
		break;
	case MENU_ANIM_OFF:
		Animate_Flag=0;
		break;
	case MENU_COPY:
		copy_object();
		break;
	case MENU_PASTE:
		paste_object();
		break;
	}
	reDisplayAll();
}

//////////help///////////////

void help_dialog(void){
	glutInitDisplayMode (GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH);
    glutInitWindowSize(500,600);
    glutInitWindowPosition(0,0); /* place window top left on display */
    glutCreateWindow("Help"); /* window title */
    glutDisplayFunc(display_help); /* display callback invoked when window opened */
	glutMouseFunc(credits_mouse);
}

void display_help(void){
glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluOrtho2D(0.0, 500.0, 0.0, 600.0);
    glMatrixMode(GL_MODELVIEW);

	glClearColor(1.0, 1.0, 1.0, 1.0); 
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glColor3ub(1, 0, 0);

	setFont("times roman",24);
	drawString(230,570,"Help");

	setFont("helvetica",12);
	glColor3f(0.0,0.0,1.0);
	drawString(10,550,"TAB : Cycle through Ojects to set Active Object");
	drawString(10,530,"O : Otrhographic View , P : Perspective View");
	drawString(10,510,"F : Full Screen , N or SpaceBar : Normal View");
	glColor3f(1.0,0.0,0.0);
	drawString(10,490,"INSERTING OBJECTS");
	glColor3f(0.0,0.0,1.0);
	drawString(10,470,"0:Cube  1:Cuboid  2:Cone  3:Sphere  4:Pyramid  5:Torus  6:Tetrahedron  7:Teapot");
	drawString(10,450,"Arrow Keys : Common Motion");
	drawString(10,430,"Enter : Reset Properties");
	glColor3f(1.0,0.0,0.0);
	drawString(10,410,"Commands in Perspective Mode");
	glColor3f(0.0,0.0,1.0);
	drawString(10,390,"+ : Zoom In");
	drawString(10,370,"- : Zoom Out");
	drawString(10,350,"= : Normal Zoom");
	glColor3f(1.0,0.0,0.0);
	drawString(10,330,"Commands in Orthographic Mode");
	glColor3f(0.0,0.0,1.0);
	drawString(10,310,"A : Translate in Positive x-axis");
	drawString(10,290,"Z : Translate in Negative x-axis");
	drawString(10,270,"X : Translate to Center");
	drawString(10,250,"E : Rotate with postive angle around x-axis");
	drawString(10,230,"D : Rotate with negative angle around x-axis");
	drawString(10,210,"C : Reset Rotation to zero");
	glColor3f(1.0,0.0,0.0);
	drawString(10,190,"Mouse Functions");
	glColor3f(0.0,0.0,1.0);
	drawString(10,170,"Right Click : Activate Menus");
	drawString(10,150,"Rotate On: Click and drag to rotate object");
	drawString(10,130,"Rotate Off: Switch off Rotation");
	drawString(10,110,"Translate: Click at new position to center object there");
	drawString(35,60,"F1=Help Hotkey  F2=Credits F3=Animate On F4=Animate Off ESC=Quit");
	drawString(150,40,"Online:http://www.irahul.com/openGL");
	glBegin(GL_QUADS);
		glColor3f(0.8,0.8,0.8);
		glVertex3f(10.0,10.0,0.0);
		glVertex3f(10.0,30.0,0.0);
		glVertex3f(490.0,30.0,0.0);
		glVertex3f(490.0,10.0,0.0);
	glEnd();
	glColor3f(0.0,0.0,0.0);
	drawString(220,15,"Click to Close");

	glutSwapBuffers();
	glutPostRedisplay();


}



///////////help end////////


///////////credits//////

void credits_mouse(int but,int state,int x,int y){
	glutDestroyWindow(glutGetWindow());
}
	
void display_credits(void){
	
	glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
    gluOrtho2D(0.0, 300.0, 0.0, 150.0);
    glMatrixMode(GL_MODELVIEW);

	glClearColor(1.0, 1.0, 1.0, 1.0); 
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glColor3ub(1, 0, 0);
	setFont("times roman",24);
	drawString(120,122,"Credits");
	setFont("times roman",10);
	drawString(122,110,"Version : 0.0.1");
	glBegin(GL_TRIANGLES);
		glColor3f(1.0,0.0,0.0);
		glVertex3f(30.0,20.0,0.0);
		glColor3f(0.0,1.0,0.0);
		glVertex3f(90.0,100.0,0.0);
		glColor3f(0.0,0.0,1.0);
		glVertex3f(150.0,20.0,0.0);
	glEnd();
	setFont("helvitica",12);
	drawString(170,80,"Samir");
	setFont("times roman",14);
	drawString(180,70,"samir@linuxturf.com");
	setFont("helvitica",12);
	drawString(170,50,"Rahul");
	setFont("times roman",14);
	drawString(180,40,"rahul@irahul.com");

	glBegin(GL_QUADS);
		glColor3f(0.8,0.8,0.8);
		glVertex3f(200.0,10.0,0.0);
		glVertex3f(270.0,10.0,0.0);
		glVertex3f(270.0,30.0,0.0);
		glVertex3f(200.0,30.0,0.0);
	glEnd();
	glColor3f(0.0,0.0,0.0);
	setFont("helvitica",12);
	drawString(220,15,"Close");


	glutSwapBuffers();
	glutPostRedisplay();

}

void credits_dialog(void){
	glutInitDisplayMode (GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH);
    glutInitWindowSize(300,150);
    glutInitWindowPosition(100,100); /* place window top left on display */
    glutCreateWindow("Credits"); /* window title */
    glutDisplayFunc(display_credits); /* display callback invoked when window opened */
	glutMouseFunc(credits_mouse);
}


///////////credits end///////


void create_menu(void){

	////////////MENU HANDLING//////////////
	
			//sub menu for srface type
			sub2 = glutCreateMenu(surface_type);
			glutAddMenuEntry("Shading",MENU_SHADING);
			glutAddMenuEntry("Wire",MENU_WIRE);		

			//sub menu for insert
			sub1 = glutCreateMenu(menu_insert);
			glutAddMenuEntry("Tetrahedron",MENU_TETR);
			glutAddMenuEntry("Cube",MENU_CUBE);
			glutAddMenuEntry("Cuboid",MENU_CUBOID);
			glutAddMenuEntry("Pyramid",MENU_PYRAMID);
			glutAddMenuEntry("Sphere",MENU_SPHERE);
			glutAddMenuEntry("Cone",MENU_CONE);
			glutAddMenuEntry("Torus",MENU_TORUS);
			glutAddMenuEntry("Teapot",MENU_TEA);

			//sub-sub menu for size
				sub4=glutCreateMenu(menu_sizex);
				glutAddMenuEntry("Largest",MENU_LARGEST);
				glutAddMenuEntry("Large",MENU_LARGE);
				glutAddMenuEntry("Medium",MENU_MEDIUM);
				glutAddMenuEntry("Small",MENU_SMALL);
				glutAddMenuEntry("Smallest",MENU_SMALLEST);


				sub5=glutCreateMenu(menu_sizey);
				glutAddMenuEntry("Largest",MENU_LARGEST);
				glutAddMenuEntry("Large",MENU_LARGE);
				glutAddMenuEntry("Medium",MENU_MEDIUM);
				glutAddMenuEntry("Small",MENU_SMALL);
				glutAddMenuEntry("Smallest",MENU_SMALLEST);

				sub6=glutCreateMenu(menu_sizez);
				glutAddMenuEntry("Largest",MENU_LARGEST);
				glutAddMenuEntry("Large",MENU_LARGE);
				glutAddMenuEntry("Medium",MENU_MEDIUM);
				glutAddMenuEntry("Small",MENU_SMALL);
				glutAddMenuEntry("Smallest",MENU_SMALLEST);

				sub7=glutCreateMenu(menu_sizeall);
				glutAddMenuEntry("Largest",MENU_LARGEST);
				glutAddMenuEntry("Large",MENU_LARGE);
				glutAddMenuEntry("Medium",MENU_MEDIUM);
				glutAddMenuEntry("Small",MENU_SMALL);
				glutAddMenuEntry("Smallest",MENU_SMALLEST);

			//sub menu for size
			sub3 = glutCreateMenu(menu_size);
			glutAddSubMenu("Resize X",sub4);
			glutAddSubMenu("Resize Y",sub5);
			glutAddSubMenu("Resize Z",sub6);
			glutAddSubMenu("Resize All",sub7);
			
			//sub menu for save environment
			sub8=glutCreateMenu(menu_save);
			glutAddMenuEntry("Save 1",MENU_SAVE1);
			glutAddMenuEntry("Save 2",MENU_SAVE2);
			glutAddMenuEntry("Save 3",MENU_SAVE3);

			sub9=glutCreateMenu(menu_load);
			glutAddMenuEntry("Load 1",MENU_LOAD1);
			glutAddMenuEntry("Load 2",MENU_LOAD2);
			glutAddMenuEntry("Load 3",MENU_LOAD3);
			

			//main menu
			glutCreateMenu(main_menu);
			glutAddSubMenu("Insert",sub1);
			glutAddSubMenu("Size",sub3);
			glutAddSubMenu("Surface",sub2);
			glutAddMenuEntry("Delete",MENU_DELETE);
			glutAddMenuEntry("Translate",MENU_TRANSLATE);
			glutAddMenuEntry("Rotate On",MENU_ROTATE);
			glutAddMenuEntry("Rotate Off",MENU_ROTATE_OFF);
			glutAddMenuEntry("Zoom in",MENU_ZOOM_IN);
			glutAddMenuEntry("Zoom out",MENU_ZOOM_OUT);
			glutAddMenuEntry("Animate On",MENU_ANIMATE);
			glutAddMenuEntry("Animate Off",MENU_ANIM_OFF);
			glutAddMenuEntry("Copy",MENU_COPY);
			glutAddMenuEntry("Paste",MENU_PASTE);
			glutAddSubMenu("Save Environment",sub8);
			glutAddSubMenu("Load Environment",sub9);
			glutAddMenuEntry("Credits",MENU_CREDITS);
			glutAddMenuEntry("Help",MENU_HELP);
			glutAddMenuEntry("Quit",MENU_QUIT);
			glutAttachMenu(GLUT_RIGHT_BUTTON);
		

	/////////////////////////MENU OVER//////////////


}

void object_init(){

	int i,j,k;
	

	for(i=0;i<MAXOBJECTS;i++){
		Object[i].obj=CUBE;
		Object[i].present=NO;
		Object[i].shad_or_wire=SHADING;
		Object[i].surfaces=6;
		for(j=0;j<6;j++)
			for(k=0;k<4;k++)
				Object[i].surf_color[j][k]=temp[j][k];
			for(j=0;j<3;j++){
				Object[i].angle[j]=0.0;
				Object[i].position[j]=0.0;
				Object[i].scale[j]=1.0;
			}
		Object[i].draw=(*drawModel[Object[i].obj]);

	}

}

int create_object(int type){
	int i,j,k;



	for(i=0;i<MAXOBJECTS;i++){
		if(Object[i].present==NO){
			Object[i].obj=type;
			Object[i].present=YES;
			Object[i].shad_or_wire=SHADING;
			Object[i].surfaces=surface(type);
			Object[i].draw=(*drawModel[type]);
			Active_Object=i;
			Num_Objects++;
			for(j=0;j<6;j++)
				for(k=0;k<4;k++)
					Object[i].surf_color[j][k]=temp[j][k];
			for(j=0;j<3;j++){
				Object[i].angle[j]=0.0;
				Object[i].position[j]=0.0;
				Object[i].scale[j]=1.0;
			}
			break;
		}
	}
	if(i==MAXOBJECTS)
		return -1;
	else
		return i;
}

int surface(int type){
	switch(type){
	case CUBE:
		return 6;
	case CUBOID:
		return 6;
	case PYRAMID:
		return 5;
	case TETRAHEDRON:
		return 4;
	case TORUS:
		return 1;
	case SPHERE:
		return 1;
	case CONE:
		return 2;
	case TEAPOT:
		return 1;
	}
	return 0;
}

void delete_object(){
	if(Active_Object!=-1){
		Object[Active_Object].present=NO;
		Num_Objects--;
	}
	Active_Object=-1;
	reDisplayAll();
}

void surface_type(int a){
	if(Active_Object!=-1){
		if(a==MENU_SHADING)
			Object[Active_Object].shad_or_wire=SHADING;
		else
			Object[Active_Object].shad_or_wire=WIRE;
	}
	reDisplayAll();
}


void menu_size(int a){


}

void menu_sizex(int a){
	if(Active_Object!=-1){
		switch(a){
		case MENU_LARGEST:
			Object[Active_Object].scale[0]=1.8;

			break;
		case MENU_LARGE:
			Object[Active_Object].scale[0]=1.4;

			break;
		case MENU_MEDIUM:
			Object[Active_Object].scale[0]=1.0;

			break;
		case MENU_SMALL:
			Object[Active_Object].scale[0]=0.6;

			break;
		case MENU_SMALLEST:
			Object[Active_Object].scale[0]=0.2;

			break;
		}
		reDisplayAll();

	}

}

void menu_sizey(int a){
	if(Active_Object!=-1){
		switch(a){
		case MENU_LARGEST:
			Object[Active_Object].scale[1]=1.8;

			break;
		case MENU_LARGE:
			Object[Active_Object].scale[1]=1.4;

			break;
		case MENU_MEDIUM:
			Object[Active_Object].scale[1]=1.0;

			break;
		case MENU_SMALL:
			Object[Active_Object].scale[1]=0.6;

			break;
		case MENU_SMALLEST:
			Object[Active_Object].scale[1]=0.2;

			break;
		}
		reDisplayAll();

	}

}

void menu_sizez(int a){
	if(Active_Object!=-1){
		switch(a){
		case MENU_LARGEST:
			Object[Active_Object].scale[2]=1.8;

			break;
		case MENU_LARGE:
			Object[Active_Object].scale[2]=1.4;

			break;
		case MENU_MEDIUM:
			Object[Active_Object].scale[2]=1.0;

			break;
		case MENU_SMALL:
			Object[Active_Object].scale[2]=0.6;

			break;
		case MENU_SMALLEST:
			Object[Active_Object].scale[2]=0.2;

			break;
		}
		reDisplayAll();

	}


}

void menu_sizeall(int a){
	if(Active_Object!=-1){
		switch(a){
		case MENU_LARGEST:
			Object[Active_Object].scale[0]=1.8;
			Object[Active_Object].scale[1]=1.8;
			Object[Active_Object].scale[2]=1.8;
			break;
		case MENU_LARGE:
			Object[Active_Object].scale[0]=1.4;
			Object[Active_Object].scale[1]=1.4;
			Object[Active_Object].scale[2]=1.4;

			break;
		case MENU_MEDIUM:
			Object[Active_Object].scale[0]=1.0;
			Object[Active_Object].scale[1]=1.0;
			Object[Active_Object].scale[2]=1.0;

			break;
		case MENU_SMALL:
			Object[Active_Object].scale[0]=0.6;
			Object[Active_Object].scale[1]=0.6;
			Object[Active_Object].scale[2]=0.6;

			break;
		case MENU_SMALLEST:
			Object[Active_Object].scale[0]=0.2;
			Object[Active_Object].scale[1]=0.2;
			Object[Active_Object].scale[2]=0.2;

			break;
		}
		reDisplayAll();

	}

}


void menu_save(int a){
	char buf[10];
	int b,i,j,k;
	
	switch(a){
	case MENU_SAVE1:
		b=1;
		break;
	case MENU_SAVE2:
		b=2;
		break;
	case MENU_SAVE3:
		b=3;
		break;
	}
	sprintf(buf,"save%d",b);
	
	if((file=fopen(buf,"w"))!=NULL){
		fwrite(Object,sizeof(Objects),MAXOBJECTS,file);
		fwrite(&Num_Objects,sizeof(int),1,file);
		
		fclose(file);
	}

}

void menu_load(int a){
	char buf[10];
	int b,i,j,k;
	
	switch(a){
	case MENU_LOAD1:
		b=1;
		break;
	case MENU_LOAD2:
		b=2;
		break;
	case MENU_LOAD3:
		b=3;
		break;
	}
	sprintf(buf,"save%d",b);
	
	if((file=fopen(buf,"r"))!=NULL){
		Active_Object=-1;
		xmotion=0;
		ymotion=0;
		Rotate_Flag=0;
		Translate_Flag=0;
		Animate_Flag=0;
		fread(&Object,sizeof(Objects),MAXOBJECTS,file);
		fread(&Num_Objects,sizeof(int),1,file);
	
		fclose(file);
	}

	reDisplayAll();

}

void animate(){
	
	
	if(Animate_Flag==1){

		if (Animate_ctr!=40){
			Animate_ctr++;
		
			switch(Animate_Key){
			case 0:
				mainSpecial(GLUT_KEY_LEFT,1,1);
				break;
			case 1:
				mainSpecial(GLUT_KEY_RIGHT,1,1);
				break;
			case 2:
				mainSpecial(GLUT_KEY_UP,1,1);
				break;
			case 3:
				mainSpecial(GLUT_KEY_DOWN,1,1);
				break;
			}
		}else{
			Animate_ctr=0;
			Animate_Key++;
			if(Animate_Key==4)
				Animate_Key=0;
		}

		

	}else if(Animate_Prev==1){
		camera_normal();
		Animate_Prev=0;
	}

	Animate_Prev=Animate_Flag;
}


void copy_object(){
	int i,j,k;

	if(Active_Object!=-1){
		for(i=0;i<3;i++){
			Copy.angle[i]=Object[Active_Object].angle[i];
			Copy.position[i]=Object[Active_Object].position[i];
			Copy.scale[i]=Object[Active_Object].scale[i];
		}
		for(i=0;i<6;i++){
			for(j=0;j<4;j++){
				Copy.surf_color[i][j]=Object[Active_Object].surf_color[i][j];
			}
		}
		Copy.obj=Object[Active_Object].obj;
		Copy.present=Object[Active_Object].present;
		Copy.shad_or_wire=Object[Active_Object].shad_or_wire;
		Copy.surfaces=Object[Active_Object].surfaces;
		Copy.draw=(*drawModel[Copy.obj]);

	}
}

void paste_object(){
	int i,j,k;
	for(i=0;i<MAXOBJECTS;i++){
		if(Object[i].present==NO){
			Object[i].obj=Copy.obj;
			Object[i].present=Copy.present;
			Object[i].shad_or_wire=Copy.shad_or_wire;
			Object[i].surfaces=surface(Copy.obj);
			Object[i].draw=(*drawModel[Copy.obj]);
			Active_Object=i;
			Num_Objects++;
			for(j=0;j<6;j++)
				for(k=0;k<4;k++)
					Object[i].surf_color[j][k]=Copy.surf_color[j][k];
			for(j=0;j<3;j++){
				Object[i].angle[j]=Copy.angle[j];
				Object[i].position[j]=Copy.position[j];
				Object[i].scale[j]=Copy.scale[j];
			}
			
		}
	}

}



