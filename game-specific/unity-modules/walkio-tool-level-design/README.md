# Module Overview

場景輸出資料時的步驟

1. Create Level Data
2. Load Design Use Master Scene and Sub Scenes
3. Create Obstacle textures and assign to Level Data

## Environment

### Main(Master) Scene

每個Level都一定會有一個Main Scene，而每個Main Scene可能會有一至多個Sub Scene。每個Sub Scene在編輯時是以Additive的方式被載入進來。

### About Sub Scenes

在手機上如果用DOTSZ給予的Sub Scene機制，是不可行的。故現階段，不論是被命名成Main(Master) Scene或是Sub Scene的unity scene資料檔，都不進Addressable，而是用之前的方式直接放在Scene List裡並進入到建置中。也因為是會放在Scene List裡，所以在引用時只需給予正確的Scene名稱即可。但也因為這個原因，不論如何Scene名稱都不可以重覆。

### Level Data

整個Level裡主要數值的部份還是放在Level Data裡。而這個資料是要進入到Addressable的。
