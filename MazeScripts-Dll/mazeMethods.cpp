#include <iostream>
#include <random>
#include <functional>
#include <algorithm>
#include <array>

#include "mazeClass.h"


Maze::Maze(){
    this->cols = 0;
    this->rows = 0;
    this->maze = nullptr;
}

Maze::Maze(unsigned int n){
    this->rows = n;
    this->cols = n;
    this->maze = new std::vector<std::vector<unsigned int>>(n, std::vector<unsigned int>(n, 0));
}

Maze::Maze(unsigned int r, unsigned int c){
    this->rows = r;
    this->cols = c;
    this->maze = new std::vector<std::vector<unsigned int>>(r, std::vector<unsigned int>(c, 0));
}


Maze::~Maze(){
    if(this->maze != nullptr){
        delete this->maze;
    }
}

void Maze::printMaze(){
    for(int i = 0; i < this->rows; i++){
        for(int j = 0; j < this->cols; j++){
            std::cout << this->maze->at(i).at(j) << " ";
        }
        std::cout << std::endl;
    }
}

void Maze::initMaze(){
    // Randomly generate start and finish points
    std::random_device rd;
    std::mt19937 gen(rd());
    std::uniform_int_distribution<> distrib(1, this->cols - 2);

    int start = 0, finish = 0;
    while(start == finish){
        // Ensure start and finish are not the same, no straight path from start to finish
        start = distrib(gen);
        finish = distrib(gen);
    }

    this->start = start;
    this->finish = finish;
    // Initialize the maze with walls around the edges
    for(int i = 0; i < this->rows; i++){
        for(int j = 0; j < this->cols; j++){
            this->maze->at(i).at(j) = WALL; // Wall
        }
    }

    this->maze->at(this->rows - 1).at(this->start) = START; // Start point
    this->maze->at(0).at(this->finish) = FINISH; // Finish point

}

bool Maze::isValid(int row, int col){
    // Check if the cell is within bounds and is a path
    return (row > 0 && row < this->rows - 1 && col > 0 && col < this->cols - 1);
}

bool Maze::checkNeighbor(int row, int col){
    // Check if the neighbor cell is a wall
    int count_walls = 0;
    if(this->maze->at(row + 1).at(col) == WALL)
        count_walls++;

    if(this->maze->at(row - 1).at(col) == WALL)
        count_walls++;

    if(this->maze->at(row).at(col + 1) == WALL)
        count_walls++;
    
    if(this->maze->at(row).at(col - 1) == WALL)
        count_walls++;

    return (count_walls == 3); // If 3 or more walls, it's a valid neighbor
}


void Maze::collectAdjacentWalls(int row, int col, std::vector<WallInfo> &frontier_list){
    const int delta_row[4] = { -1, 1, 0, 0 };
    const int delta_col[4] = {  0, 0, -1, 1 };

    for (int d = 0; d < 4; ++d) {
        int wallR = row + delta_row[d];
        int wallC = col + delta_col[d];
        int nextR = row + 2 * delta_row[d];
        int nextC = col + 2 * delta_col[d];

        if(isValid(nextR, nextC) && maze->at(nextR).at(nextC) == WALL){
            frontier_list.push_back({wallR, wallC, nextR, nextC});
        }
    }
}

// Generates a random maze using a modified Prim's algorithm
void Maze::createMazePrims(int start_row, int start_col, std::mt19937 &gen)
{
    // These help us move in 4 directions: N, S, W, E
    const int delta_row[4] = {-1, 1, 0, 0};
    const int delta_col[4] = { 0, 0, -1, 1};

    std::vector<WallInfo> frontier_list;

    // Start by carving out the initial cell
    maze->at(start_row).at(start_col) = PATH;

    collectAdjacentWalls(start_row, start_col, frontier_list);  // fill in the first set of walls
    std::uniform_int_distribution<std::size_t> rand_index;  // this lets us pick walls at random

    // Begin the actual maze generation
    while(!frontier_list.empty()){
        // Randomly pick one of the frontier walls
        rand_index.param(std::uniform_int_distribution<std::size_t>::param_type(0, frontier_list.size() - 1));
        std::size_t chosen_idx = rand_index(gen);
        WallInfo chosen_wall = frontier_list[chosen_idx];

        // Erase the selected wall (we just swap with the last to avoid moving everything)
        if(chosen_idx != frontier_list.size() - 1){
            frontier_list[chosen_idx] = frontier_list.back();
        }
        frontier_list.pop_back();

        // If the next cell hasn’t been carved yet, we knock down the wall and continue
        if(maze->at(chosen_wall.next_row).at(chosen_wall.next_col) == WALL){
            maze->at(chosen_wall.wall_row).at(chosen_wall.wall_col) = PATH;
            maze->at(chosen_wall.next_row).at(chosen_wall.next_col) = PATH;

            // Recursively collect new walls from this newly carved cell
            collectAdjacentWalls(chosen_wall.next_row, chosen_wall.next_col, frontier_list);
        }
        // Otherwise, we've already been here. Just skip this wall
    }

}

void Maze::generateMaze(){
    // 1. lay down a solid border & clear interior
    initMaze();                     

    // 2. make every interior cell a wall first   
    for (unsigned r=1;r<rows-1;++r)
        for (unsigned c=1;c<cols-1;++c)
            maze->at(r).at(c) = WALL;

    std::mt19937 gen(std::random_device{}());

    // 3. carve a perfect maze starting two rows above the start
    int sr = rows - 2, sc = start;
    maze->at(sr).at(sc) = PATH;
    createMazePrims(sr, sc, gen);

    // 4. ensure finish cell is connected
    // If it's still a wall, tunnel straight down until we hit a PATH.
    if (maze->at(1).at(finish) == WALL) {
        int r = 1;
        maze->at(r).at(finish) = PATH;
        while (r < rows-1 && checkNeighbor(r, finish)) {
            r++;
            maze->at(r).at(finish) = PATH;
        }
    }

    maze->at(rows-1).at(start) = START;
    maze->at(0).at(finish) = FINISH;

}

int Maze::getMazeValue(int row, int col){
    return this->maze->at(row).at(col);
}