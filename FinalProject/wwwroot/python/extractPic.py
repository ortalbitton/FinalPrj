import cv2
import numpy as np
import sys

folder = sys.argv[1] #folder path
videoName =sys.argv[2] #video name
#jumps = sys.argv[3] #frame jumps
totalFrames =sys.argv[3] #sum of all video frames


cap = cv2.VideoCapture(folder+videoName)
counter = 0


while (cap.isOpened()):
#while (counter>=int(totalFrames)):
    #frame_no = float(int(counter) /int(totalFrames))
    #cap.set(2,frame_no)

    success, frame = cap.read()
    if success == False:
      print(0)
      break
   
    cv2.imwrite(folder + '/thumb' +str(counter)+ '.jpg', frame)
    counter +=1

    if cv2.waitKey(10) == 27:                     # exit if Escape is hit
      print(str(counter))
      cap.release()

    #if (counter>=int(totalFrames)):
      #print(str(counter))
      #cap.release()

print(str(counter))

#success,image = cap.read()
#if success == True:
   # return
 #  cv2.imwrite(folder+"frame.jpg", image)     # save frame as JPEG file
  # print("5")
   #if cv2.waitKey(10) == 27:                     # exit if Escape is hit
    #  cap.release()

#i = 0

#cap.release()

cap.release()

