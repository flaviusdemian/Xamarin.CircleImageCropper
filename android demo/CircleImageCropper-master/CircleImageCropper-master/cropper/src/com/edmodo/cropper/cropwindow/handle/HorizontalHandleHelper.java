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
 * Handle helper class to handle horizontal handles (i.e. top and bottom
 * handles).
 */
class HorizontalHandleHelper extends HandleHelper {

    // Member Variables ////////////////////////////////////////////////////////

    private EdgeOrientation mEdge;
    private float coordinate;

    // Constructor /////////////////////////////////////////////////////////////

    HorizontalHandleHelper(EdgeOrientation edge, float coordinate) {
        super(edge, EdgeOrientation.NONE, coordinate, 0);
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
        final float targetWidth = AspectRatioUtil.calculateWidth(top, bottom, targetAspectRatio);
        final float currentWidth = right - left;

        // Adjust the crop window so that it maintains the given aspect ratio by
        // moving the adjacent edges symmetrically in or out.
        final float difference = targetWidth - currentWidth;
        final float halfDifference = difference / 2;
        left -= halfDifference;
        right += halfDifference;

        EdgeType.LEFT = left;
        EdgeType.RIGHT = right;

        // Check if we have gone out of bounds on the sides, and fix.
        if (Edge.isOutsideMargin(imageRect, snapRadius, EdgeType.LEFT, EdgeOrientation.LEFT) 
        		&& !Edge.isNewRectangleOutOfBounds(mEdge, imageRect, targetAspectRatio, coordinate, EdgeOrientation.LEFT)) 
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.LEFT, EdgeOrientation.LEFT);
            EdgeType.RIGHT = Edge.offset(-offset, EdgeType.RIGHT);
            coordinate = Edge.adjustCoordinate(targetAspectRatio, mEdge);

        }
        if (Edge.isOutsideMargin(imageRect, snapRadius, EdgeType.RIGHT, EdgeOrientation.RIGHT) && 
        		!Edge.isNewRectangleOutOfBounds(mEdge, imageRect, targetAspectRatio, coordinate, EdgeOrientation.RIGHT)) 
        {
            final float offset = Edge.snapToRect(imageRect, EdgeType.RIGHT, EdgeOrientation.RIGHT);
            EdgeType.LEFT = Edge.offset(-offset, EdgeType.LEFT);
            coordinate = Edge.adjustCoordinate(targetAspectRatio, mEdge);
        }
    }
}
