//
//  CaptureScreenshots.m
//  Unity-iPhone
//
//  Created by Russ Lunsford on 6/11/11.
//  Copyright 2011 Meticulous Technology, Inc. All rights reserved.
//

#import "CaptureScreenshots.h"
#import "UIImage-Extensions.h"

@implementation CaptureScreenshots
void releaseData(void *info, const void *data, size_t dataSize)
{
    free((void*)data);
}

+ (void)image:(UIImage *)image didFinishSavingWithError:(NSError *)error contextInfo:(void *)contextInfo
{
    [image release];
}

+ (void)saveCurrentScreenToPhotoAlbum:(bool)lscapeRight
{
    CGRect rect = [[UIScreen mainScreen] bounds];
    int width = rect.size.width;
    int height = rect.size.height;
    
    UIScreen* screen = [UIScreen mainScreen];
    width = screen.currentMode.size.height;
    height = screen.currentMode.size.width;

    if (width < height)
    {
        int tmp = height;
        height = width;
        width = tmp;
    }

    
    NSLog(@"Width: %d, Height: %d", width, height);
    
    NSInteger myDataLength = width * height * 4;
    GLubyte *buffer = (GLubyte *) malloc(myDataLength);
    GLubyte *buffer2 = (GLubyte *) malloc(myDataLength);
    glReadPixels(0, 0, width, height, GL_RGBA, GL_UNSIGNED_BYTE, buffer);
	
    for(int y = 0; y <height; y++) 
	{
        for(int x = 0; x <width * 4; x++)
		{
            buffer2[int((height - 1 - y) * width * 4 + x)] = buffer[int(y * 4 * width + x)];
        }
    }		

    free(buffer);
    CGDataProviderRef provider = CGDataProviderCreateWithData(NULL, buffer2, myDataLength, releaseData);
    int bitsPerComponent = 8;
    int bitsPerPixel = 32;
    int bytesPerRow = 4 * width;
    CGColorSpaceRef colorSpaceRef = CGColorSpaceCreateDeviceRGB();
    CGBitmapInfo bitmapInfo = kCGBitmapByteOrderDefault;
    CGColorRenderingIntent renderingIntent = kCGRenderingIntentDefault;
    CGImageRef imageRef = CGImageCreate(width, height, bitsPerComponent, bitsPerPixel, bytesPerRow, colorSpaceRef, bitmapInfo, provider, NULL, NO, renderingIntent);
    CGColorSpaceRelease(colorSpaceRef);
    CGDataProviderRelease(provider);
	
    UIImage *image = [[[UIImage alloc] initWithCGImage:imageRef] imageRotatedByDegrees:lscapeRight ? 180 : 0];
	//UIImage *image = [[UIImage alloc] initWithCGImage:imageRef];
	
    CGImageRelease(imageRef);
    UIImageWriteToSavedPhotosAlbum(image, self, (SEL)@selector(image:didFinishSavingWithError:contextInfo:), nil);
}

@end

extern "C"
{
	void CaptureScreenshotToCameraRoll (bool lscapeRight)
	{
		NSLog(@"...native function\n");
        
        [CaptureScreenshots saveCurrentScreenToPhotoAlbum:lscapeRight];
        
	}
} 
