/*
 * Copyright 2013, Edmodo, Inc. 
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this work except in compliance with the License.
 * You may obtain a copy of the License in the LICENSE file, or at:
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" 
 * BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language 
 * governing permissions and limitations under the License. 
 */

package com.edmodo.cropper.cropwindow.handle;

import android.graphics.Rect;

import com.edmodo.cropper.cropwindow.edge.EdgeManager;

/**
 * HandleHelper class to handle the center handle.
 */
class CenterHandleHelper extends HandleHelper 
{
    // Constructor /////////////////////////////////////////////////////////////

    CenterHandleHelper() 
    {
        super(null, null);
    }

    // HandleHelper Methods ////////////////////////////////////////////////////

    @Override
    void updateCropWindow(float x,
                          float y,
                          Rect imageRect,
                          float snapRadius) 
    {

        float left = EdgeManager.LEFT.coordinate;
        float top = EdgeManager.TOP.coordinate;
        float right = EdgeManager.RIGHT.coordinate;
        float bottom = EdgeManager.BOTTOM.coordinate;

        final float currentCenterX = (left + right) / 2;
        final float currentCenterY = (top + bottom) / 2;

        final float offsetX = x - currentCenterX;
        final float offsetY = y - currentCenterY;

        // Adjust the crop window.
        EdgeManager.LEFT.offset(offsetX);
        EdgeManager.TOP.offset(offsetY);
        EdgeManager.RIGHT.offset(offsetX);
        EdgeManager.BOTTOM.offset(offsetY);

        // Check if we have gone out of bounds on the sides, and fix.
        if (EdgeManager.LEFT.isOutsideMargin(imageRect, snapRadius)) 
        {
            final float offset = EdgeManager.LEFT.snapToRect(imageRect);
            EdgeManager.RIGHT.offset(offset);
        }
        else 
    	if (EdgeManager.RIGHT.isOutsideMargin(imageRect, snapRadius)) 
    	{
            final float offset = EdgeManager.RIGHT.snapToRect(imageRect);
            EdgeManager.LEFT.offset(offset);
        }

        // Check if we have gone out of bounds on the top or bottom, and fix.
        if (EdgeManager.TOP.isOutsideMargin(imageRect, snapRadius)) 
        {
            final float offset = EdgeManager.TOP.snapToRect(imageRect);
            EdgeManager.BOTTOM.offset(offset);
        } 
        else 
    	if (EdgeManager.BOTTOM.isOutsideMargin(imageRect, snapRadius)) 
    	{
            final float offset = EdgeManager.BOTTOM.snapToRect(imageRect);
            EdgeManager.TOP.offset(offset);
        }
    }

    @Override
    void updateCropWindow(float x,
                          float y,
                          float targetAspectRatio,
                          Rect imageRect,
                          float snapRadius) {

        updateCropWindow(x, y, imageRect, snapRadius);
    }
}
