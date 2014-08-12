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
import com.edmodo.cropper.cropwindow.edge.EdgePair;

/**
 * HandleHelper class to handle corner Handles (i.e. top-left, top-right,
 * bottom-left, and bottom-right handles).
 */
class CornerHandleHelper extends HandleHelper {

    // Constructor /////////////////////////////////////////////////////////////

    CornerHandleHelper(EdgeOrientation horizontalEdge, EdgeOrientation verticalEdge, float horizontalCoordinate, float verticalCoordinate) {
        super(horizontalEdge, verticalEdge, horizontalCoordinate, verticalCoordinate);
    }

    // HandleHelper Methods ////////////////////////////////////////////////////

    @Override
    void updateCropWindow(float x,
                          float y,
                          float targetAspectRatio,
                          Rect imageRect,
                          float snapRadius) {

        final EdgePair activeEdges = getActiveEdges(x, y, targetAspectRatio);
        EdgeOrientation primaryEdge = activeEdges.primary;
        EdgeOrientation secondaryEdge = activeEdges.secondary;
        float primaryCoordinate = activeEdges.primaryCoordinate;
        float secondaryCoordinate = activeEdges.secondaryCoordinate;

        primaryCoordinate = Edge.adjustCoordinate(x, y, imageRect, snapRadius, targetAspectRatio, primaryEdge);
        secondaryCoordinate = Edge.adjustCoordinate(targetAspectRatio, secondaryEdge);

        if (Edge.isOutsideMargin(imageRect, snapRadius, secondaryCoordinate, secondaryEdge)) {
        	secondaryCoordinate = Edge.snapToRect(imageRect, secondaryCoordinate, secondaryEdge);
        	primaryCoordinate = Edge.adjustCoordinate(targetAspectRatio, primaryEdge);
        }
    }
}
