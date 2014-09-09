/*######################################################################################
 *
 * Stitch
 * ------
 * Panoramic Image, Image Mosaic, Image Registration
 *
 * Copyright:       (c) 2002 Rahul
 * email:            rahul@irahul.com
 * 
 * This is free software; you can redistribute it and/or modify it under the 
 * terms of the GNU General Public Liscence as published by Free Software Foundation;
 * either version 2 of the liscence, or (at your option) any later version.
 *
 * You are free to use and distribute this program keeping this header intact.
 *
 *######################################################################################
 */

#include <stdio.h>
#include <stdlib.h>
#include "cv.h"        /* Basic OpenCV header file. */
#include "highgui.h"   /* Additional support for reading and writing image file. */
#include <gtk/gtk.h>
#include <string.h>
#define MAX_NUMBER_OF_CORNERS 200
#define MAX_IMAGES 20
#define HALF_WIDTH 1
//variable declarations
GtkWidget *window;
GtkWidget *table;
GtkWidget *event1,*event2;
GtkWidget *pic1,*pic2;
GdkPixmap *pix1,*pix2;
GdkBitmap *mask1,*mask2;
GtkStyle *s1,*s2;
int WIDTH=400,HEIGHT=300;
int gap=0;
char *xpmimg1,*xpmimg2;
char ch;
float a11,a12,a13,a21,a22,a23;
FILE *fin,*fout;
int numImages;
int i,j,x,y,xnew,ynew;
IplImage *images[MAX_IMAGES], *gray[MAX_IMAGES],*colorimg[MAX_IMAGES];
IplImage *final,*showimg1,*showimg2;
CvSize size[MAX_IMAGES];
IplImage *image, *image1, *gray1, *eigImage1, *tempImg1, *eigImage2, *tempImg2, *velx, *vely,*save;
CvSize size1,winSize,fsize,savesize;
CvMat *p,*p1x,*u,*v,*pc,*p1y;
CvPoint2D32f corners[MAX_NUMBER_OF_CORNERS],newpoints[MAX_NUMBER_OF_CORNERS];
int cornerCount,mycolor=1;int col;
double qualityLevel, minDistance;
unsigned char *row,*column,*row0,*rowp1,*rowp2,*rown1,*rown2;
float pdata[MAX_NUMBER_OF_CORNERS][3]; 
float p1xdata[MAX_NUMBER_OF_CORNERS][1];
float p1ydata[MAX_NUMBER_OF_CORNERS][1];
float pcdata[MAX_NUMBER_OF_CORNERS][3];
float udata[MAX_NUMBER_OF_CORNERS][3];
float vdata[MAX_NUMBER_OF_CORNERS][3];
int xdash, ydash,xdash1,ydash1;
int forInter[1000][2];
int forInterCounter=0;
int picp1[MAX_NUMBER_OF_CORNERS][2],picp2[MAX_NUMBER_OF_CORNERS][2];
int clickcount=0,clicker=0;
int c0x,c0y,c1x,c1y,maxydash1=0,minydash1=9999,maxxdash1=0,minxdash1=9999;//to keep track of corners to crop image
int col0,colp1,colp2,coln1,coln2;
int blurCounter=0;
int finalImageColumnCounter=0;
int finalImageRowCounter=0;
int first=0,second=1,mergecount; //to keep track of multi images
char* ppmfile1,*xpmfile1,*ppmfile2,*xpmfile2;
int cmddashb=0,cmddashi=1,cmddashe=1,cmddashz=0;//0=disabled,1=enabled, for z ->0=>going behind, 1=>going front
char *chrdashb="-b",*chrdashi="-i",*chrdashe="-e",*chrdashF="-F",*chrdashz="-z";
char *outfilename="output.jpg";
//fn prototypes
void doMerge();
void crop();
void blur();
void markImages();
void showImages();
void interpolate();
void interpolateHelp();
void hapakkeInterpolate();
void hapakkeInterpolateEngine();
int ppmquant(char*,char*,int);
int ppmtoxpm(char*,char*);

char* ppm2xpm(char* filename){
    char tmp1[50],tmp2[50],*output;
    sprintf(tmp1,"/tmp/stitch.%ld",random());
    ppmquant(filename,tmp1,256);
    sprintf(tmp2,"/tmp/stitch.%ld",random());
    ppmtoxpm(tmp1,tmp2);

    output=(char*)malloc(sizeof(tmp2));
    strcpy(output,tmp2);
    return output;
}

void mark(IplImage *image, int x, int y)
{
   int r, c, p;
   unsigned char *row, *pixel;
   int red, green, blue;
   red = 0; green = 255; blue = 0;
   /* Get pointer to row y of image (assumed origin at top-left corner). */
   row = image->imageData + image->widthStep * y;
   /* Draw box. */
   for (r = y - HALF_WIDTH; r <= y + HALF_WIDTH; ++r)
   {
      pixel = row;  /* Point to first byte of row. */
      p = (x - HALF_WIDTH) * 3;  
      for (c = x - HALF_WIDTH; c <= x + HALF_WIDTH; ++c)
	{
          pixel[p++] = blue;
          pixel[p++] = green;
          pixel[p++] = red;
	}
      row = row + image->widthStep;   /* Point at next row. */
   }
}

int delete_event( GtkWidget *widget,GdkEvent  *event,gpointer   data )
{
  return FALSE;
} 
void destroy( GtkWidget *widget,gpointer   data )
{
  gtk_main_quit ();
}

static gint
button_press_event1 (GtkWidget *widget, GdkEventButton *event)
{

     printf("Choosing Point set:%d\n",(clickcount/9)+1);
      picp1[clickcount][0]=(int)event->x;
      picp1[clickcount][1]=size[first].height-(int)event->y;
      //select 9 points for every click
      picp1[clickcount+1][0]=picp1[clickcount][0]-1;
      picp1[clickcount+1][1]=picp1[clickcount][1]-1;
      picp1[clickcount+2][0]=picp1[clickcount][0];
      picp1[clickcount+2][1]=picp1[clickcount][1]-1;
      picp1[clickcount+3][0]=picp1[clickcount][0]+1;
      picp1[clickcount+3][1]=picp1[clickcount][1]-1;
      picp1[clickcount+4][0]=picp1[clickcount][0]-1;
      picp1[clickcount+4][1]=picp1[clickcount][1];
      picp1[clickcount+5][0]=picp1[clickcount][0]+1;
      picp1[clickcount+5][1]=picp1[clickcount][1];
      picp1[clickcount+6][0]=picp1[clickcount][0]-1;
      picp1[clickcount+6][1]=picp1[clickcount][1]+1;
      picp1[clickcount+7][0]=picp1[clickcount][0];
      picp1[clickcount+7][1]=picp1[clickcount][1]+1;
      picp1[clickcount+8][0]=picp1[clickcount][0]+1;
      picp1[clickcount+8][1]=picp1[clickcount][1]+1;

      printf("Now click corresponding point in Image 2\n");
      clicker++;
      if(clicker%2==0) clickcount+=9;//one pair of points for 2 clicks

}

static gint
button_press_event2 (GtkWidget *widget, GdkEventButton *event)
{
     printf("Choosing Point set:%d\n",(clickcount/9)+1);
      picp2[clickcount][0]=(int)event->x;
      picp2[clickcount][1]=size[second].height-(int)event->y;

      picp2[clickcount+1][0]=picp2[clickcount][0]-1;
      picp2[clickcount+1][1]=picp2[clickcount][1]-1;
      picp2[clickcount+2][0]=picp2[clickcount][0];
      picp2[clickcount+2][1]=picp2[clickcount][1]-1;
      picp2[clickcount+3][0]=picp2[clickcount][0]+1;
      picp2[clickcount+3][1]=picp2[clickcount][1]-1;
      picp2[clickcount+4][0]=picp2[clickcount][0]-1;
      picp2[clickcount+4][1]=picp2[clickcount][1];
      picp2[clickcount+5][0]=picp2[clickcount][0]+1;
      picp2[clickcount+5][1]=picp2[clickcount][1];
      picp2[clickcount+6][0]=picp2[clickcount][0]-1;
      picp2[clickcount+6][1]=picp2[clickcount][1]+1;
      picp2[clickcount+7][0]=picp2[clickcount][0];
      picp2[clickcount+7][1]=picp2[clickcount][1]+1;
      picp2[clickcount+8][0]=picp2[clickcount][0]+1;
      picp2[clickcount+8][1]=picp2[clickcount][1]+1;

      printf("Choose a point in Image 1\n");
      clicker++;
      if(clicker%2==0) clickcount+=9;//one pair of points for 2 clicks

}


main (int argc, char **argv)
{ 
  gtk_init(&argc,&argv);
  ppm_init(&argc,argv);
  cornerCount=MAX_NUMBER_OF_CORNERS;
  
  if (argc < 2)
    {
      //command line options
      fprintf (stderr, "Usage: stitch <num of images> <image names...> [options]\n
  -b   Enable Averaging on common area upon merging, note that this reduces quality if more than two images: Default Disabled.\n
  -i   Disable Interpolation in merged image: Default Enabled.\n
  -e   Disable Edge Blurring of merged image (For 2 Images only): Default Enabled.\n
  -z   If given second image come on top of first: Deafult (Goes Behind)\n
  -F   Specify output filename, it MUST end with .jpg: Default is output.jpg\n
  Example: stitch 2 picA.jpg picB.jpg -F merged.jpg -b -e -i\n
  \n ");
      exit (0);
    }
  
  /* load images */
  // now all images are available in gray[] and colorimg[]
  numImages=atoi(argv[1]);
  
  printf("Intializing...\n");
  
  for(i=0;i<numImages;i++)
    {
      printf("Loading Image %d....",i+1);
      images[i]=cvvLoadImage(argv[2+i]);
      size[i].width=images[i]->width;
      size[i].height=images[i]->height;
      gray[i]=cvCreateImage(size[i],IPL_DEPTH_8U, 1);
      colorimg[i]=cvCreateImage(size[i],IPL_DEPTH_8U,3);
      cvvConvertImage(images[i],gray[i],0);/*coverts 3-channel image to 1 channel image*/
      cvvConvertImage(images[i],colorimg[i],0);//3-channel rgb color images
      printf("done\n");
    }

  /* translate command line options */
  for(j=2+numImages;j<argc;j++){
    if(strcmp(argv[j],chrdashb)==0) cmddashb=1;
    if(strcmp(argv[j],chrdashi)==0) cmddashi=0;
    if(strcmp(argv[j],chrdashe)==0) cmddashe=0;
    if(strcmp(argv[j],chrdashF)==0) outfilename=argv[++j];
    if(strcmp(argv[j],chrdashz)==0) cmddashz=1;
  }
  
  printf("Merging with Averaging(Blur)=");
  if(cmddashb) printf("Enabled\n"); else printf("Disabled\n");
  printf("Merging with Interpolation=");
  if(cmddashi) printf("Enabled\n"); else printf("Disabled\n");
  printf("Merging with Edge Blurring=");
  if(cmddashe) printf("Enabled\n"); else printf("Disabled\n");
  printf("Merging with Second Image ");
  if(cmddashz==1) printf("on top\n"); else printf("going behind\n");
  printf("\n");
  /* control multiple image merge */ 

  for(mergecount=0;mergecount<numImages-1;mergecount++)
    {
      first=0;
      second=1;
      if(mergecount>0){
	//gary,colorimg,size
	cvReleaseImage(&gray[first]);
	cvReleaseImage(&colorimg[first]);

	size[first].width=savesize.width;
	size[first].height=savesize.height;
	gray[first]=cvCreateImage(size[first],IPL_DEPTH_8U,1);
	cvvConvertImage(save,gray[first],0);
	colorimg[first]=cvCreateImage(size[first],IPL_DEPTH_8U,3);
	cvvConvertImage(save,colorimg[first],0);
	second=mergecount+1;

	cvReleaseImage(&final);
	cvReleaseImage(&save);
      }

      doMerge();
      crop();
    }

  printf("\nWriting Image to JPEG file: %s\n",outfilename);
  //write final image to file
  cvvSaveImage(outfilename,save);
  //done!
}


void doMerge()
{

  /*
    -Get corresponding points between images
    -Determine affine transformation between corresponding points
    -Merge images
  */
  forInterCounter=0;
  printf("Marking Images....");
    markImages();
  printf("done\n");
  //find matching  points by showing pictures to user
  //
  //customize window values
  WIDTH=size[first].width+size[second].width+40;
  HEIGHT=size[first].height;
  ppmfile1="mark1.ppm";
  ppmfile2="mark2.ppm";
  xpmfile1=ppm2xpm(ppmfile1);
  xpmfile2=ppm2xpm(ppmfile2);
  clicker=0; clickcount=0;
  printf("Please choose corresponding points between images\n");
  printf("Choose a point in Image 1\n");
  //
  //Show images to user and get points
  //
  showImages();
  //
  //Write to file for matlab processing
  //
  fout=fopen("fromc.txt","w");
  fprintf(fout,"%d\n",clickcount);
  for(i=0;i<clickcount;i++)
    {
      fprintf(fout,"%d\n",picp2[i][0]);
      fprintf(fout,"%d\n",picp2[i][1]);
      fprintf(fout,"%d\n",picp1[i][0]);
      fprintf(fout,"%d\n",picp1[i][1]);
    }
  fclose(fout); 
  //
  //Read file created by matlab
  //
  printf("Run Matlab and then press enter to continue:\n");
  scanf("%c",&ch);
  fin=fopen("toc.txt","r");
  fscanf(fin,"%f",&a11);
  fscanf(fin,"%f",&a12);
  fscanf(fin,"%f",&a13);
  fscanf(fin,"%f",&a21);
  fscanf(fin,"%f",&a22);
  fscanf(fin,"%f",&a23);
  fclose(fin);
  //
  //found affine transformation
  //
  maxydash1=0;minydash1=9999;maxxdash1=0;minxdash1=9999;//define max extent for image cropping  
  //merge images
  //
  //copy first image
  fsize.width=size[first].width+2*size[second].width;
  fsize.height=size[first].height+2*size[second].height;
  final=cvCreateImage(fsize,IPL_DEPTH_8U,3);//temp image where two images merge into
  for(i=0;i<size[first].height;i++)//copy first image into final
    {
      for(j=0;j<size[first].width;j++)
	{
	  row=(final->imageData + final->widthStep*(size[second].height+i));
	  col=size[second].width+j;
	  row0 = (colorimg[first]->imageData + colorimg[first]->widthStep*i);
	  row[3*col]=row0[3*j];
	  row[3*col+1]=row0[3*j+1];
	  row[3*col+2]=row0[3*j+2];
	}
    }

  //cropping area
  c0x=size[second].width;//upper left corner
  c0y=size[second].height;
  c1x=size[second].width+size[first].width;//lower right corner
  c1y=size[second].height+size[first].height;
  //merge second image

  for(i=0;i<size[second].height;i++)
    {
      for(j=0;j<size[second].width;j++)
	{
	  row=(colorimg[second]->imageData + colorimg[second]->widthStep*(size[second].height-i));
	  xdash=(int) (a11*j+a12*i+a13);
	  ydash=(int) (a21*j+a22*i+a23);
	  
	  xdash1=size[second].width+xdash;
	  ydash1=size[second].height+size[first].height-ydash;

	  if(xdash1>maxxdash1) maxxdash1=xdash1;//find image extent to crop
	  if(xdash1<minxdash1) minxdash1=xdash1;
	  if(ydash1>maxydash1) maxydash1=ydash1;
	  if(ydash1<minydash1) minydash1=ydash1;

	  if(ydash1>0 && ydash1<2*size[second].height+size[first].height)
	    {
	      row0=(final->imageData + final->widthStep*ydash1);
	    }
	  if(xdash1>0 && xdash1<2*size[second].width+size[first].width)
	    {
	      if(cmddashz==1){//second image on top
	      //The following takes care of averaging (blurring)
	     	  row0[3*xdash1]=row[3*j];
		  row0[3*xdash1+1]=row[3*j+1];
		  row0[3*xdash1+2]=row[3*j+2];
		 
		  if(cmddashb){//if requested on command line only then it does the averaging
		    row0[3*xdash1]=(row[3*j]+row0[3*xdash1])/2;
		    row0[3*xdash1+1]=(row[3*j+1]+row0[3*xdash1+1])/2;
		    row0[3*xdash1+2]=(row[3*j+2]+row0[3*xdash1+2])/2;
		   }
		 
	      }
	      else{//first image on top
		//The following takes care of averaging (blurring)
	        if(row0[3*xdash1]==0&&row0[3*xdash1+1]==0&&row0[3*xdash1+2]==0)
	        {
		  row0[3*xdash1]=row[3*j];
		  row0[3*xdash1+1]=row[3*j+1];
		  row0[3*xdash1+2]=row[3*j+2];
		 }
		 else
		 {
		  if(cmddashb){//if requested on command line only then it does the averaging
		    row0[3*xdash1]=(row[3*j]+row0[3*xdash1])/2;
		    row0[3*xdash1+1]=(row[3*j+1]+row0[3*xdash1+1])/2;
		    row0[3*xdash1+2]=(row[3*j+2]+row0[3*xdash1+2])/2;
		   }
		  }
	      }
	    }
	  // The following stores the coordinates to blur
	    if(i==0||j==0)
	    {
	      forInter[forInterCounter][0]=xdash1;
	      forInter[forInterCounter][1]=ydash1;
	      forInterCounter++;
	    }
	}             
    }
  if(cmddashe==1 && second==1) {blur();} //blur edge (as required by command line parameter)
  if(cmddashi==1)
    {
      interpolate(); //interpolate unless disabled by command line parameter
    }
}

void crop()
{

  if(minxdash1<c0x) c0x=minxdash1;
  if(maxxdash1>c1x) c1x=maxxdash1;
  if(minydash1<c0y) c0y=minydash1;
  if(maxydash1>c1y) c1y=maxydash1;
  savesize.width=c1x-c0x;
  savesize.height=c1y-c0y;

  finalImageRowCounter=0;
  finalImageColumnCounter=0;

  save=cvCreateImage(savesize,IPL_DEPTH_8U,3);
  for(i=c0y;i<c1y;i++)
    {
      row0=(final->imageData + final->widthStep*i);
      
      rown1=(save->imageData + save->widthStep*finalImageRowCounter);
      finalImageRowCounter++;
      finalImageColumnCounter=0;
      for(j=c0x;j<c1x;j++)
	{
	  rown1[3*finalImageColumnCounter]=row0[3*j];
	  rown1[3*finalImageColumnCounter+1]=row0[3*j+1];
	  rown1[3*finalImageColumnCounter+2]=row0[3*j+2];
	  finalImageColumnCounter++;	  
	}
    }

}

void blur()
{
  //The following is edge blurring.
  i=size[second].height;
  row0=(final->imageData + final->widthStep*i);
  rowp1=(final->imageData + final->widthStep*(i-1));
  rowp2=(final->imageData + final->widthStep*(i-2));
  rown1=(final->imageData + final->widthStep*(i+1));
  rown2=(final->imageData + final->widthStep*(i+2));
  for(j=size[second].width;j<size[second].width+size[first].width;j++)
    {
      if(rowp1[3*j]!=0&&rowp1[3*j+1]!=0&&rowp1[3*j+2]!=0)
	{
	   if(rowp2[3*j]!=0&&rowp2[3*j+1]!=0&&rowp2[3*j+2]!=0)
	     {
	       row0[3*j]=(int) (row0[3*j]+rowp1[3*j]+rowp2[3*j]+rown1[3*j]+rown2[3*j])/5;
	       row0[3*j+1]=(int) (row0[3*j+1]+rowp1[3*j+1]+rowp2[3*j+1]+rown1[3*j+1]+rown2[3*j+1])/5;
	       row0[3*j+2]=(int) (row0[3*j+2]+rowp1[3*j+2]+rowp2[3*j+2]+rown1[3*j+2]+rown2[3*j+2])/5;
	       rowp1[3*j]=(int) (row0[3*j]+rowp1[3*j]+rowp2[3*j]+rown1[3*j]+rown2[3*j])/5;
	       rowp1[3*j+1]=(int) (row0[3*j+1]+rowp1[3*j+1]+rowp2[3*j+1]+rown1[3*j+1]+rown2[3*j+1])/5;
	       rowp1[3*j+2]=(int) (row0[3*j+2]+rowp1[3*j+2]+rowp2[3*j+2]+rown1[3*j+2]+rown2[3*j+2])/5;
	       rown1[3*j]=(int) (row0[3*j]+rowp1[3*j]+rowp2[3*j]+rown1[3*j]+rown2[3*j])/5;
	       rown1[3*j+1]=(int) (row0[3*j+1]+rowp1[3*j+1]+rowp2[3*j+1]+rown1[3*j+1]+rown2[3*j+1])/5;
	       rown1[3*j+2]=(int) (row0[3*j+2]+rowp1[3*j+2]+rowp2[3*j+2]+rown1[3*j+2]+rown2[3*j+2])/5;
	     }
	}
    }

  i=size[second].height+size[first].height;
  row0=(final->imageData + final->widthStep*i);
  rowp1=(final->imageData + final->widthStep*(i-1));
  rowp2=(final->imageData + final->widthStep*(i-2));
  rown1=(final->imageData + final->widthStep*(i+1));
  rown2=(final->imageData + final->widthStep*(i+2));
  for(j=size[second].width;j<size[second].width+size[first].width;j++)
    {
      if(rown1[3*j]!=0&&rown1[3*j+1]!=0&&rown1[3*j+2]!=0)
	{
	   if(rown2[3*j]!=0&&rown2[3*j+1]!=0&&rown2[3*j+2]!=0)
	     {
	       row0[3*j]=(int) (row0[3*j]+rowp1[3*j]+rowp2[3*j]+rown1[3*j]+rown2[3*j])/5;
	       row0[3*j+1]=(int) (row0[3*j+1]+rowp1[3*j+1]+rowp2[3*j+1]+rown1[3*j+1]+rown2[3*j+1])/5;
	       row0[3*j+2]=(int) (row0[3*j+2]+rowp1[3*j+2]+rowp2[3*j+2]+rown1[3*j+2]+rown2[3*j+2])/5;
	       rowp1[3*j]=(int) (row0[3*j]+rowp1[3*j]+rowp2[3*j]+rown1[3*j]+rown2[3*j])/5;
	       rowp1[3*j+1]=(int) (row0[3*j+1]+rowp1[3*j+1]+rowp2[3*j+1]+rown1[3*j+1]+rown2[3*j+1])/5;
	       rowp1[3*j+2]=(int) (row0[3*j+2]+rowp1[3*j+2]+rowp2[3*j+2]+rown1[3*j+2]+rown2[3*j+2])/5;
	       rown1[3*j]=(int) (row0[3*j]+rowp1[3*j]+rowp2[3*j]+rown1[3*j]+rown2[3*j])/5;
	       rown1[3*j+1]=(int) (row0[3*j+1]+rowp1[3*j+1]+rowp2[3*j+1]+rown1[3*j+1]+rown2[3*j+1])/5;
	       rown1[3*j+2]=(int) (row0[3*j+2]+rowp1[3*j+2]+rowp2[3*j+2]+rown1[3*j+2]+rown2[3*j+2])/5;
	     }
	}
    }
  
  for(i=size[second].height;i<size[second].height+size[first].height;i++)
    {
      row0=(final->imageData + final->widthStep*i);
      col0=size[second].width;
      colp1=size[second].width-1;
      colp2=size[second].width-2;
      coln1=size[second].width+1;
      coln2=size[second].width+2;
      if(row0[3*colp1]!=0&&row0[3*colp1+1]!=0&&row0[3*colp1+2]!=0)
	{
	   if(row0[3*colp2]!=0&&row0[3*colp2+1]!=0&&row0[3*colp2+2]!=0)
	     {
	       row0[3*col0]=(int) (row0[3*col0]+row0[3*colp1]+row0[3*colp2]+row0[3*coln1]+row0[3*coln2])/5;
	       row0[3*col0+1]=(int) (row0[3*col0+1]+row0[3*colp1+1]+row0[3*colp2+1]+row0[3*coln1+1]+row0[3*coln2+1])/5;
	       row0[3*col0+2]=(int) (row0[3*col0+2]+row0[3*colp1+2]+row0[3*colp2+2]+row0[3*coln1+2]+row0[3*coln2+2])/5;
	        row0[3*colp1]=(int) (row0[3*col0]+row0[3*colp1]+row0[3*colp2]+row0[3*coln1]+row0[3*coln2])/5;
	       row0[3*colp1+1]=(int) (row0[3*col0+1]+row0[3*colp1+1]+row0[3*colp2+1]+row0[3*coln1+1]+row0[3*coln2+1])/5;
	       row0[3*colp1+2]=(int) (row0[3*col0+2]+row0[3*colp1+2]+row0[3*colp2+2]+row0[3*coln1+2]+row0[3*coln2+2])/5;
	        row0[3*coln1]=(int) (row0[3*col0]+row0[3*colp1]+row0[3*colp2]+row0[3*coln1]+row0[3*coln2])/5;
	       row0[3*coln1+1]=(int) (row0[3*col0+1]+row0[3*colp1+1]+row0[3*colp2+1]+row0[3*coln1+1]+row0[3*coln2+1])/5;
	       row0[3*coln1+2]=(int) (row0[3*col0+2]+row0[3*colp1+2]+row0[3*colp2+2]+row0[3*coln1+2]+row0[3*coln2+2])/5;
	     }
	}

    }

  for(i=size[second].height;i<size[second].height+size[first].height;i++)
    {
      row0=(final->imageData + final->widthStep*i);
      col0=size[second].width+size[first].width;
      colp1=size[second].width+size[first].width-1;
      colp2=size[second].width+size[first].width-2;
      coln1=size[second].width+size[first].width+1;
      coln2=size[second].width+size[first].width+2;
      if(row0[3*coln1]!=0&&row0[3*coln1+1]!=0&&row0[3*coln1+2]!=0)
	{
	   if(row0[3*coln2]!=0&&row0[3*coln2+1]!=0&&row0[3*coln2+2]!=0)
	     {
	       row0[3*col0]=(int) (row0[3*col0]+row0[3*colp1]+row0[3*colp2]+row0[3*coln1]+row0[3*coln2])/5;
	       row0[3*col0+1]=(int) (row0[3*col0+1]+row0[3*colp1+1]+row0[3*colp2+1]+row0[3*coln1+1]+row0[3*coln2+1])/5;
	       row0[3*col0+2]=(int) (row0[3*col0+2]+row0[3*colp1+2]+row0[3*colp2+2]+row0[3*coln1+2]+row0[3*coln2+2])/5;
	       row0[3*colp1]=(int) (row0[3*col0]+row0[3*colp1]+row0[3*colp2]+row0[3*coln1]+row0[3*coln2])/5;
	       row0[3*colp1+1]=(int) (row0[3*col0+1]+row0[3*colp1+1]+row0[3*colp2+1]+row0[3*coln1+1]+row0[3*coln2+1])/5;
	       row0[3*colp1+2]=(int) (row0[3*col0+2]+row0[3*colp1+2]+row0[3*colp2+2]+row0[3*coln1+2]+row0[3*coln2+2])/5;
	       row0[3*coln1]=(int) (row0[3*col0]+row0[3*colp1]+row0[3*colp2]+row0[3*coln1]+row0[3*coln2])/5;
	       row0[3*coln1+1]=(int) (row0[3*col0+1]+row0[3*colp1+1]+row0[3*colp2+1]+row0[3*coln1+1]+row0[3*coln2+1])/5;
	       row0[3*coln1+2]=(int) (row0[3*col0+2]+row0[3*colp1+2]+row0[3*colp2+2]+row0[3*coln1+2]+row0[3*coln2+2])/5;
	     }
	}

    }
    

}
	     
void markImages(){

  //Mark features found and mark on image for user to confirm   
  //Good Features to track
  eigImage1 = cvCreateImage (size[first], IPL_DEPTH_32F, 1); /* 1 channel floating */
  tempImg1 = cvCreateImage (size[first], IPL_DEPTH_32F, 1); /* 1 channel floating temp */
  eigImage2 = cvCreateImage (size[second], IPL_DEPTH_32F, 1); /* 1 channel floating */
  tempImg2 = cvCreateImage (size[second], IPL_DEPTH_32F, 1); /* 1 channel floating temp */
  showimg1=cvCreateImage(size[first],IPL_DEPTH_8U,3);
  showimg2=cvCreateImage(size[second],IPL_DEPTH_8U,3);
  cvvConvertImage(colorimg[first],showimg1,0);
  cvvConvertImage(colorimg[second],showimg2,0);
  qualityLevel=.4;
  minDistance=5;
  
  cvGoodFeaturesToTrack(gray[first], eigImage1, tempImg1, corners, &cornerCount, qualityLevel, minDistance, NULL);

  for(i = 0; i < cornerCount; i++)
    {
      mark(showimg1, corners[i].x, corners[i].y);
    }
  
  cvGoodFeaturesToTrack(gray[second], eigImage2, tempImg2, corners, &cornerCount, qualityLevel, minDistance, NULL);
  
  for(i = 0; i < cornerCount; i++)
    {
      mark(showimg2, corners[i].x, corners[i].y);
    }
  
  cvvSaveImage ("mark1.ppm", showimg1);
  cvvSaveImage ("mark2.ppm", showimg2);      


}

void showImages()
{
  
  window = gtk_window_new (GTK_WINDOW_TOPLEVEL);
  gtk_window_set_title(GTK_WINDOW(window),"Stitch");
  gtk_widget_set_usize(window,WIDTH,HEIGHT); 
  gtk_window_set_default_size(GTK_WINDOW(window),WIDTH,HEIGHT); 
  
  gtk_signal_connect(GTK_OBJECT (window), "delete_event",
		     GTK_SIGNAL_FUNC (delete_event), NULL);
  
  gtk_signal_connect(GTK_OBJECT (window), "destroy",
		     GTK_SIGNAL_FUNC(destroy), NULL);
  
  table=gtk_table_new(1,2,TRUE);
  gtk_container_add(GTK_CONTAINER(window),table);
  
  
  s1=gtk_widget_get_style(window);
  s2=gtk_widget_get_style(window);
  pix1 = gdk_pixmap_create_from_xpm( window->window, &mask1,
				     &s1->bg[GTK_STATE_NORMAL],xpmfile1 );
  pix2 = gdk_pixmap_create_from_xpm( window->window, &mask2,
				     &s2->bg[GTK_STATE_NORMAL],xpmfile2 );
  
  free(xpmfile1);//important to free the filenames of xpms afterwards
  free(xpmfile2);//same as above
  
  pic1=gtk_pixmap_new(pix1,mask1);
  pic2=gtk_pixmap_new(pix2,mask2);
  
  event1=gtk_event_box_new();
  event2=gtk_event_box_new();
  gtk_container_add(GTK_CONTAINER(event1),pic1);
  gtk_container_add(GTK_CONTAINER(event2),pic2);
  gtk_table_attach(GTK_TABLE(table),event1,0,1,0,1,GTK_EXPAND,GTK_EXPAND,gap,0);
  gtk_table_attach(GTK_TABLE(table),event2,1,2,0,1,GTK_EXPAND,GTK_EXPAND,gap,0);
    
  gtk_widget_set_events( window,gtk_widget_get_events( event1 ) |
			 GDK_BUTTON_PRESS_MASK );
  gtk_signal_connect( GTK_OBJECT(event1), "button_press_event",
		      GTK_SIGNAL_FUNC(button_press_event1), NULL );
  gtk_widget_set_events( window,gtk_widget_get_events( event2 ) |
			 GDK_BUTTON_PRESS_MASK );
  gtk_signal_connect( GTK_OBJECT(event2), "button_press_event",
		      GTK_SIGNAL_FUNC(button_press_event2), NULL );
  
  gtk_widget_show(event1);
  gtk_widget_show(event2);
  gtk_widget_show(pic2);
  gtk_widget_show(pic1);
  gtk_widget_show(table);
  gtk_widget_show(window);
  
  gtk_main();
  //
  //end user interface
  //
}

void interpolate()
{
  int i1=0,i2=0;

  for(i = minydash1; i < maxydash1; i++)
    {
      row0=(final->imageData + final->widthStep*i);
      rowp1=(final->imageData + final->widthStep*(i-1));
      rown1=(final->imageData + final->widthStep*(i+1));
      rowp2=(final->imageData + final->widthStep*(i-2));
      rown2=(final->imageData + final->widthStep*(i+2));

      for(j = minxdash1; j < maxxdash1; j++)
	{
	  for(i1=0;i1<forInterCounter;i1++){
	    if(forInter[i1][0]==j && forInter[i1][1]==i){
	      //do nothing
	    }
	    else{
	      hapakkeInterpolateEngine();
	      break;
	    }
	  }   
	} 
    } 
}

void hapakkeInterpolateEngine()
{
 
  if(row0[3*j]==0&&row0[3*j+1]==0&&row0[3*j+2]==0)
    {
       hapakkeInterpolate();
    }

}

void interpolateHelp()
{
  int v1=1;
  int v2=1;
  int h1=1;
  int h2=1;
  int emptyDiagonalCounter=0;
  int emptyStraightCounter=0;

  //the following code finds out how many of the diagonal neighbours are themselves empty
  if(rowp1[3*(j-1)]==0&&rowp1[3*(j-1)+1]==0&&rowp1[3*(j-1)+2]==0)
    emptyDiagonalCounter++;
  if(rowp1[3*(j+1)]==0&&rowp1[3*(j+1)+1]==0&&rowp1[3*(j+1)+2]==0)
    emptyDiagonalCounter++;
  if(rown1[3*(j-1)]==0&&rown1[3*(j-1)+1]==0&&rown1[3*(j-1)+2]==0)
    emptyDiagonalCounter++;
  if(rown1[3*(j+1)]==0&&rown1[3*(j+1)+1]==0&&rown1[3*(j+1)+2]==0)
    emptyDiagonalCounter++;
  //*******************************************************************

  if(row0[3*j]==0&&row0[3*j+1]==0&&row0[3*j+2]==0&&emptyDiagonalCounter<2)
    {
      row0[3*j]=(h1*v1*rowp1[3*(j+1)]+h1*v2*rown1[3*(j+1)]+h2*v1*rowp1[3*(j-1)]+h2*v2*rown1[3*(j+1)])/(h1+h2);
      row0[3*j+1]=(h1*v1*rowp1[3*(j+1)+1]+h1*v2*rown1[3*(j+1)+1]+h2*v1*rowp1[3*(j-1)+1]+h2*v2*rown1[3*(j+1)+1])/(h1+h2);
      row0[3*j+2]=(h1*v1*rowp1[3*(j+1)+2]+h1*v2*rown1[3*(j+1)+2]+h2*v1*rowp1[3*(j-1)+2]+h2*v2*rown1[3*(j+1)+2])/(h1+h2);
    }

   //the following code finds out how many of the straight neighbours are themselves empty
  if(rowp1[3*j]==0&&rowp1[3*j+1]==0&&rowp1[3*j+2]==0)
    emptyStraightCounter++;
  if(row0[3*(j+1)]==0&&row0[3*(j+1)+1]==0&&row0[3*(j+1)+2]==0)
    emptyStraightCounter++;
  if(row0[3*(j-1)]==0&&row0[3*(j-1)+1]==0&&row0[3*(j-1)+2]==0)
    emptyStraightCounter++;
  if(rown1[3*j]==0&&rown1[3*j+1]==0&&rown1[3*j+2]==0)
    emptyStraightCounter++;
  //*******************************************************************
  if(row0[3*j]==0&&row0[3*j+1]==0&&row0[3*j+2]==0&&emptyStraightCounter<2)
    {
      row0[3*j]=(row0[3*(j-1)]+row0[3*(j+1)]+rowp1[3*j]+rown1[3*j])/4;
      row0[3*j+1]=(row0[3*(j-1)+1]+row0[3*(j+1)+1]+rowp1[3*j+1]+rown1[3*j+1])/4;
      row0[3*j+2]=(row0[3*(j-1)+2]+row0[3*(j+1)+2]+rowp1[3*j+2]+rown1[3*j+2])/4;
    }
}

void hapakkeInterpolate()
{
  
  int divide=0;
  float rtotal=0,gtotal=0,btotal=0;
  float w=0.7;
  //printf("happake\n");
  //*******************1 point*********************
  if(rowp1[3*(j-1)]==0&&rowp1[3*(j-1)+1]==0&&rowp1[3*(j-1)+2]==0)
    {
      if(rowp2[3*(j-2)]==0&&rowp2[3*(j-2)+1]==0&&rowp2[3*(j-2)+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*rowp2[3*(j-2)];
	  gtotal=gtotal+w*rowp2[3*(j-2)+1];
	  btotal=btotal+w*rowp2[3*(j-2)+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+rowp1[3*(j-1)];
      gtotal=gtotal+rowp1[3*(j-1)+1];
      btotal=btotal+rowp1[3*(j-1)+2];
      divide++;
  }
  //***************1 point ends***************************

  //******************2 point starts***********************
    if(rowp1[3*j]==0&&rowp1[3*j+1]==0&&rowp1[3*j+2]==0)
    {
      if(rowp2[3*j]==0&&rowp2[3*j+1]==0&&rowp2[3*j+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*rowp2[3*j];
	  gtotal=gtotal+w*rowp2[3*j+1];
	  btotal=btotal+w*rowp2[3*j+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+rowp1[3*j];
      gtotal=gtotal+rowp1[3*j+1];
      btotal=btotal+rowp1[3*j+2];
      divide++;
  }
   
   //****************2 point ends************************

   //****************3 point starts**********************
    if(rowp1[3*(j+1)]==0&&rowp1[3*(j+1)+1]==0&&rowp1[3*(j+1)+2]==0)
    {
      if(rowp2[3*(j+2)]==0&&rowp2[3*(j+2)+1]==0&&rowp2[3*(j+2)+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*rowp2[3*(j+2)];
	  gtotal=gtotal+w*rowp2[3*(j+2)+1];
	  btotal=btotal+w*rowp2[3*(j+2)+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+rowp1[3*(j+1)];
      gtotal=gtotal+rowp1[3*(j+1)+1];
      btotal=btotal+rowp1[3*(j+1)+2];
      divide++;
  }
   //****************3 point ends************************

   //****************4 point starts*********************
    if(row0[3*(j-1)]==0&&row0[3*(j-1)+1]==0&&row0[3*(j-1)+2]==0)
    {
      if(row0[3*(j-2)]==0&&row0[3*(j-2)+1]==0&&row0[3*(j-2)+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*row0[3*(j-2)];
	  gtotal=gtotal+w*row0[3*(j-2)+1];
	  btotal=btotal+w*row0[3*(j-2)+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+row0[3*(j-1)];
      gtotal=gtotal+row0[3*(j-1)+1];
      btotal=btotal+row0[3*(j-1)+2];
      divide++;
   }
   //****************4 point ends***********************

   //****************5 point starts*********************
    if(row0[3*(j+1)]==0&&row0[3*(j+1)+1]==0&&row0[3*(j+1)+2]==0)
    {
      if(row0[3*(j+2)]==0&&row0[3*(j+2)+1]==0&&row0[3*(j+2)+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*row0[3*(j+2)];
	  gtotal=gtotal+w*row0[3*(j+2)+1];
	  btotal=btotal+w*row0[3*(j+2)+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+row0[3*(j+1)];
      gtotal=gtotal+row0[3*(j+1)+1];
      btotal=btotal+row0[3*(j+1)+2];
      divide++;
  }
   //****************5 point ends***********************

   //****************6 point starts*********************
    if(rown1[3*(j-1)]==0&&rown1[3*(j-1)+1]==0&&rown1[3*(j-1)+2]==0)
    {
      if(rown2[3*(j-2)]==0&&rown2[3*(j-2)+1]==0&&rown2[3*(j-2)+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*rown2[3*(j-2)];
	  gtotal=gtotal+w*rown2[3*(j-2)+1];
	  btotal=btotal+w*rown2[3*(j-2)+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+rown1[3*(j-1)];
      gtotal=gtotal+rown1[3*(j-1)+1];
      btotal=btotal+rown1[3*(j-1)+2];
      divide++;
   }
   //****************6 point ends***********************


   //****************7 point starts*********************
    if(rown1[3*j]==0&&rown1[3*j+1]==0&&rown1[3*j+2]==0)
    {
      if(rown2[3*j]==0&&rown2[3*j+1]==0&&rown2[3*j+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*rown2[3*j];
	  gtotal=gtotal+w*rown2[3*j+1];
	  btotal=btotal+w*rown2[3*j+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+rown1[3*j];
      gtotal=gtotal+rown1[3*j+1];
      btotal=btotal+rown1[3*j+2];
      divide++;
   }
   //****************7 point ends***********************


   //****************8 point starts*********************
    if(rown1[3*(j+1)]==0&&rown1[3*(j+1)+1]==0&&rown1[3*(j+1)+2]==0)
    {
      if(rown2[3*(j+2)]==0&&rown2[3*(j+2)+1]==0&&rown2[3*(j+2)+2]==0)
	{
	  
	}
      else
	{
	  rtotal=rtotal+w*rown2[3*(j+2)];
	  gtotal=gtotal+w*rown2[3*(j+2)+1];
	  btotal=btotal+w*rown2[3*(j+2)+2];
	  divide++;
	}
    }
  else
  {
      rtotal=rtotal+rown1[3*(j+1)];
      gtotal=gtotal+rown1[3*(j+1)+1];
      btotal=btotal+rown1[3*(j+1)+2];
      divide++;
  }
   //****************8 point ends***********************
 
   
   if(rtotal!=0&&gtotal!=0&&btotal!=0&&divide>4)
     {
       row0[3*j]=rtotal/divide;
       row0[3*j+1]=gtotal/divide;
       row0[3*j+2]=btotal/divide;
     }
}

