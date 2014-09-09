% This is help MATLAB  program used for our Image Resgistration Project

numpoints=0;
fid = fopen('fromc.txt','rt');
numpoints=str2num(fgetl(fid));

oldpoints=zeros(numpoints,3);
newpointsx=zeros(numpoints,1);
newpointsy=zeros(numpoints,1);

for i=1:numpoints
   
   temp=str2num(fgetl(fid));
   %temp
   oldpoints(i,1)=temp;
   
   temp=str2num(fgetl(fid));
   oldpoints(i,2)=temp;
   
   oldpoints(i,3)=1;
   
   temp=str2num(fgetl(fid));
   newpointsx(i,1)=temp;
   
   temp=str2num(fgetl(fid));
   newpointsy(i,1)=temp;
   
end
fclose(fid); 

[Q1,R1] = qr(oldpoints,0); 
x = R1\(Q1'*newpointsx);
x %a11 a12 a13

[Q1,R1] = qr(oldpoints,0); 
y = R1\(Q1'*newpointsy);
y %a21 a22 a23

fid=fopen('toc.txt','wt');
fprintf(fid, '%s\n',num2str(x(1,1)));
fprintf(fid, '%s\n',num2str(x(2,1)));
fprintf(fid, '%s\n',num2str(x(3,1)));
fprintf(fid, '%s\n',num2str(y(1,1)));
fprintf(fid, '%s\n',num2str(y(2,1)));
fprintf(fid, '%s\n',num2str(y(3,1)));
fclose(fid);   %close file

!rm fromc.txt -f