
#include "mazeClass.h"
#include <string>
#include <sstream>

extern "C" {
    __declspec(dllexport) const char* GenerateMaze(int rows, int cols) {
        static std::string output;

        Maze m(rows, cols);
        m.generateMaze();

        std::ostringstream oss;
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j) {
                oss << m.getMazeValue(i, j);  // See note below
            }
            oss << "\n";
        }

        output = oss.str();
        return output.c_str();
    }
}




