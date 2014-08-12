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
 * HandleHelper class to handle vertical handles (i.e. left and right handles).
 */
class VerticalHandleHelper extends HandleHelper {

    // Member Variables ////////////////////////////////////////////////////////

    private Edge mEdge;

    // Constructor /////////////////////////////////////////////////////////////

    VerticalHandleHelper(Edge edge) {
        super(null, edge);
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
        final float targetHeight = AspectRatioUtil.calculateHeight(left, right, targetAspectRatio);
        final float currentHeight = bottom - top;

        // Adjust the crop window so that it maintains the given aspect ratio by
        // moving the adjacent edges symmetrically in or out.
        final float difference = targetHeight - currentHeight;
        final float halfDifference = difference / 2;
        top -= halfDifference;
        bottom += halfDifference;

        EdgeManager.TOP.coordinate = top;
        EdgeManager.BOTTOM.coordinate = bottom;

        // Check if we have gone out of bounds on the top or bottom, and fix.
        if (EdgeManager.TOP.isOutsideMargin(imageRect, snapRadius) && 
        		!mEdge.isNewRectangleOutOfBounds(EdgeManager.TOP, imageRect, targetAspectRatio)) 
        {
            final float offset = EdgeManager.TOP.snapToRect(imageRect);
            EdgeManager.BOTTOM.offset(-offset);
            mEdge.adjustCoordinate(targetAspectRatio);
        }
        if (EdgeManager.BOTTOM.isOutsideMargin(imageRect, snapRadius) && 
        		!mEdge.isNewRectangleOutOfBounds(EdgeManager.BOTTOM, imageRect,targetAspectRatio)) 
        {
            final float offset = EdgeManager.BOTTOM.snapToRect(imageRect);
            EdgeManager.TOP.offset(-offset);
            mEdge.adjustCoordinate(targetAspectRatio);
        }
    }
}
