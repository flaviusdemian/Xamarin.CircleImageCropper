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
import com.edmodo.cropper.util.AspectRatioUtil;

/**
 * HandleHelper class to handle vertical handles (i.e. left and right handles).
 */
class VerticalHandleHelper extends HandleHelper {

    // Member Variables ////////////////////////////////////////////////////////

    private EdgeOrientation mEdge;
    private float coordinate;
    // Constructor /////////////////////////////////////////////////////////////

    VerticalHandleHelper(EdgeOrientation edge, float coordinate) {
        super(EdgeOrientation.NONE, edge, 0 , coordinate);
        mEdge = edge;
        this.coordinate = coordinate;
    }

    // HandleHelper Methods ////////////////////////////////////////////////////

    @Override
    void updateCropWindow(float x,
                          float y,
                          float targetAspectRatio,
                          Rect imageRect,
                          float snapRadius) {

        // Adjust this Edge accordingly.
        coordinate = Edge.adjustCoordinate(x, y, imageRect, snapRadius, targetAspectRatio, mEdge);

        float left = EdgeType.LEFT;
        float top = EdgeType.TOP;
        float right = EdgeType.RIGHT;
        float bottom = EdgeType.BOTTOM;

        // After this Edge is moved, our crop window is now out of proportion.
        final float targetHeight = AspectRatioUtil.calculateHeight(left, right, targetAspectRatio);
        final float currentHeight = bottom - top;

        // Adjust the crop window so that it maintains the given aspect ratio by
        // moving the adjacent edges symmetrically in or out.
        final float difference = targetHeight - currentHeight;
        final float halfDifference = difference / 2;
        top -= halfDifference;
        bottom += halfDifference;

        EdgeType.TOP = top;
        EdgeType.BOTTOM = bottom;

        // Check if we have gone out of bounds on the top or bottom, and fix.
        if (Edge.isOutsideMargin(imageRect, snapRadius, EdgeType.TOP, EdgeOrientation.TOP) && 
    		!Edge.isNewRectangleOutOfBounds(mEdge, imageRect, targetAspectRatio, coordinate, EdgeOrientation.TOP))
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.TOP, EdgeOrientation.TOP);
            EdgeType.BOTTOM = Edge.offset(-offset, EdgeType.BOTTOM);
            coordinate= Edge.adjustCoordinate(targetAspectRatio, mEdge);
        }
        if (Edge.isOutsideMargin(imageRect, snapRadius,EdgeType.BOTTOM, EdgeOrientation.BOTTOM) && 
    		!Edge.isNewRectangleOutOfBounds(mEdge, imageRect, targetAspectRatio, coordinate, EdgeOrientation.BOTTOM))
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.BOTTOM, EdgeOrientation.BOTTOM);
            EdgeType.TOP = Edge.offset(-offset, EdgeType.TOP);
            coordinate = Edge.adjustCoordinate(targetAspectRatio, mEdge);
        }
    }
}
