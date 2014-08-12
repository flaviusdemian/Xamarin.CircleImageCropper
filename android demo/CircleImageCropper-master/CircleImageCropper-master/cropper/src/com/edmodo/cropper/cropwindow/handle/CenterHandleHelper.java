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

import com.edmodo.cropper.cropwindow.edge.Edge;
import com.edmodo.cropper.cropwindow.edge.EdgeOrientation;
import com.edmodo.cropper.cropwindow.edge.EdgeType;

/**
 * HandleHelper class to handle the center handle.
 */
class CenterHandleHelper extends HandleHelper {

    // Constructor /////////////////////////////////////////////////////////////

    CenterHandleHelper() {
        super(EdgeOrientation.NONE, EdgeOrientation.NONE, 0, 0);
    }

    // HandleHelper Methods ////////////////////////////////////////////////////

    @Override
    void updateCropWindow(float x,
                          float y,
                          Rect imageRect,
                          float snapRadius) {

        float left = EdgeType.LEFT;
        float top = EdgeType.TOP;
        float right = EdgeType.RIGHT;
        float bottom = EdgeType.BOTTOM;

        final float currentCenterX = (left + right) / 2;
        final float currentCenterY = (top + bottom) / 2;

        final float offsetX = x - currentCenterX;
        final float offsetY = y - currentCenterY;

        // Adjust the crop window.
        EdgeType.LEFT = Edge.offset(offsetX, EdgeType.LEFT);
        EdgeType.TOP = Edge.offset(offsetY, EdgeType.TOP);
        EdgeType.RIGHT = Edge.offset(offsetX, EdgeType.RIGHT);
        EdgeType.BOTTOM = Edge.offset(offsetY, EdgeType.BOTTOM);

        // Check if we have gone out of bounds on the sides, and fix.
        if (Edge.isOutsideMargin(imageRect, snapRadius, EdgeType.LEFT, EdgeOrientation.LEFT))
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.LEFT, EdgeOrientation.LEFT);
            EdgeType.RIGHT = Edge.offset(offset, EdgeType.RIGHT);
        } else 
    	if (Edge.isOutsideMargin(imageRect, snapRadius, EdgeType.RIGHT, EdgeOrientation.RIGHT)) 
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.RIGHT, EdgeOrientation.RIGHT);
            EdgeType.LEFT = Edge.offset(offset,EdgeType.LEFT);
        }

        // Check if we have gone out of bounds on the top or bottom, and fix.
        if (Edge.isOutsideMargin(imageRect, snapRadius, EdgeType.TOP, EdgeOrientation.TOP)) 
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.TOP, EdgeOrientation.TOP);
            EdgeType.BOTTOM = Edge.offset(offset, EdgeType.BOTTOM);
        } else 
    	if (Edge.isOutsideMargin(imageRect, snapRadius, EdgeType.BOTTOM , EdgeOrientation.BOTTOM)) 
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.BOTTOM, EdgeOrientation.BOTTOM);
            EdgeType.TOP = Edge.offset(offset, EdgeType.TOP);
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
