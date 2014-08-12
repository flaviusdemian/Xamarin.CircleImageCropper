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
import com.edmodo.cropper.cropwindow.edge.EdgeManager;
import com.edmodo.cropper.util.AspectRatioUtil;

/**
 * Handle helper class to handle horizontal handles (i.e. top and bottom
 * handles).
 */
class HorizontalHandleHelper extends HandleHelper {

    // Member Variables ////////////////////////////////////////////////////////

    private Edge mEdge;

    // Constructor /////////////////////////////////////////////////////////////

    HorizontalHandleHelper(Edge edge) {
        super(edge, null);
        mEdge = edge;
    }

    // HandleHelper Methods ////////////////////////////////////////////////////

    @Override
    void updateCropWindow(float x,
                          float y,
                          float targetAspectRatio,
                          Rect imageRect,
                          float snapRadius) {

        // Adjust this Edge accordingly.
        mEdge.adjustCoordinate(x, y, imageRect, snapRadius, targetAspectRatio);

        float left = EdgeManager.LEFT.coordinate;
        float top = EdgeManager.TOP.coordinate;
        float right = EdgeManager.RIGHT.coordinate;
        float bottom = EdgeManager.BOTTOM.coordinate;

        // After this Edge is moved, our crop window is now out of proportion.
        final float targetWidth = AspectRatioUtil.calculateWidth(top, bottom, targetAspectRatio);
        final float currentWidth = right - left;

        // Adjust the crop window so that it maintains the given aspect ratio by
        // moving the adjacent edges symmetrically in or out.
        final float difference = targetWidth - currentWidth;
        final float halfDifference = difference / 2;
        left -= halfDifference;
        right += halfDifference;

        EdgeManager.LEFT.coordinate = left;
        EdgeManager.RIGHT.coordinate = right;

        // Check if we have gone out of bounds on the sides, and fix.
        if (EdgeManager.LEFT.isOutsideMargin(imageRect, snapRadius) && 
        		!mEdge.isNewRectangleOutOfBounds(EdgeManager.LEFT, imageRect, targetAspectRatio)) 
        {
            final float offset = EdgeManager.LEFT.snapToRect(imageRect);
            EdgeManager.RIGHT.offset(-offset);
            mEdge.adjustCoordinate(targetAspectRatio);
        }
        if (EdgeManager.RIGHT.isOutsideMargin(imageRect, snapRadius) && 
        		!mEdge.isNewRectangleOutOfBounds(EdgeManager.RIGHT, imageRect, targetAspectRatio)) 
        {
            final float offset = EdgeManager.RIGHT.snapToRect(imageRect);
            EdgeManager.LEFT.offset(-offset);
            mEdge.adjustCoordinate(targetAspectRatio);
        }
    }
}
