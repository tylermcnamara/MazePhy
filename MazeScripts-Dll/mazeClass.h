#ifndef MAZECLASS_H
#define MAZECLASS_H

#include <vector>
#include <random>

enum CellType {
    WALL = 0, 
    PATH = 1, 
    START = 2, 
    FINISH = 3 
};

struct WallInfo {
    int wallRow, wallCol;
    int nextRow, nextCol;
};

// Maze class: A simple representation of a maze using a 2D vector
class Maze{

    private:
        unsigned int rows;
        unsigned int cols;
        std::vector<std::vector<unsigned int>> *maze;

        void initMaze();

        bool isValid(int row, int col);
        bool checkNeighbor(int row, int col);
        void collectAdjacentWalls(int row, int col, std::vector<WallInfo>& frontierList);
        void createMazePrims(int row, int col, std::mt19937 &gen);

    public:
        int start;
        int finish;
        Maze();
        Maze(unsigned int n);
        Maze(unsigned int r, unsigned int c);
        ~Maze();

        void printMaze();
        int getMazeValue(int row, int col);

        void generateMaze();
        


};

#endif // MAZECLASS_H