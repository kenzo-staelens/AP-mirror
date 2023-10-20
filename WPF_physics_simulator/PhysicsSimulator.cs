using System;
using System.Collections.Generic;
using System.Linq;
using Globals;
using Components;

namespace WPF_physics_simulator {
    public class PhysicsSimulator {
        public Rect[] Environment;
        public Ball PhysicsEntity;

        private static readonly double Elasticity = 0.2;
        private static readonly double g = 9.81; // zwaartekracht cte
        private static readonly double PhysicsMass =2e7;
        private static readonly double minimumCollisionVelocity = 1e-2; //used to unstuck on edges

        private readonly int CellCountHeight;
        private readonly int CellSize;

        public PhysicsSimulator(Rect[] environment, Ball ball, Maze maze, int cellsize) {
            this.Environment = environment; //assumption walls are immovable
            this.PhysicsEntity = ball;

            //optimization data
            this.CellCountHeight = maze.Height;
            this.CellSize = cellsize;
        }

        private T ConvertNullable<T>(object? nullable, T default_obj) {
            if (nullable == null) {
                return default_obj;
            }
            return (T)nullable;
        }

        public PhysicsComponent Simulate(double AngleX, double AngleY, long dt_millis) {
            PhysicsComponent physicsComponent = ConvertNullable(PhysicsEntity.GetComponent(typeof(PhysicsComponent)), new PhysicsComponent());

            physicsComponent.Force.X = g * Math.Sin(AngleX);
            physicsComponent.Force.Y = g * Math.Sin(AngleY);

            int cellIndex_width = (int)Math.Floor(PhysicsEntity.X / CellSize);
            int cellIndex_height = (int)Math.Floor(PhysicsEntity.Y / CellSize);
            int cellIndex = cellIndex_height + cellIndex_width*CellCountHeight;

            int[] validIndexes = new int[] {
                cellIndex - CellCountHeight - 1,
                cellIndex - CellCountHeight    ,
                cellIndex - CellCountHeight + 1,
                cellIndex                   - 1,
                cellIndex                      ,
                cellIndex                   + 1,
                cellIndex + CellCountHeight - 1,
                cellIndex + CellCountHeight    ,
                cellIndex + CellCountHeight + 1
            };

            List<Rect> collidingRects = new();
            for(int i = 0; i < this.Environment.Length; i++) {
                Environment[i].collides(false);
                if (!validIndexes.Contains<int>(Environment[i].source_cell)){
                    Environment[i].mark(false);
                    continue;//wall too far to check
                }
                Environment[i].mark(true);
                if (CollidesWith(PhysicsEntity, Environment[i])){
                    collidingRects.Add(Environment[i]);//ball collides with wall
                    Environment[i].collides(true);
                }
            }

            CalculateCollisionVectors(collidingRects, physicsComponent);

            PhysicsEntity.X += physicsComponent.Velocity.X * dt_millis + physicsComponent.Acceleration.X * dt_millis * dt_millis / 2;
            PhysicsEntity.Y += physicsComponent.Velocity.Y * dt_millis + physicsComponent.Acceleration.X * dt_millis * dt_millis / 2;
            physicsComponent.Velocity.X += physicsComponent.Acceleration.X * dt_millis;
            physicsComponent.Velocity.Y += physicsComponent.Acceleration.Y * dt_millis;
            physicsComponent.Acceleration.X += physicsComponent.Force.X * dt_millis / PhysicsMass;
            physicsComponent.Acceleration.Y += physicsComponent.Force.Y * dt_millis / PhysicsMass;
            return physicsComponent;//for printing statistics in debug mode
        }

        private void CalculateCollisionVectors(List<Rect> collidingRects, PhysicsComponent physics) {
            bool collidesTop = false;
            bool collidesRight = false;
            bool collidesBottom = false;
            bool collidesLeft = false;
            if (collidingRects.Count == 0) return;
            for (int i = 0; i < collidingRects.Count; i++) {
                if (PhysicsEntity.Y <= collidingRects[i].y1) { collidesTop = true; }
                if (PhysicsEntity.Y >= collidingRects[i].y2) { collidesBottom = true; }
                if (PhysicsEntity.X >= collidingRects[i].x2) { collidesRight = true; }
                if (PhysicsEntity.X <= collidingRects[i].x1) { collidesLeft= true;}
            }

            if (collidesTop) {
                //flip Y axis
                if(physics.Velocity.Y>0) physics.Velocity.Y = Math.Min(-physics.Velocity.Y * Elasticity,-minimumCollisionVelocity);
                physics.Acceleration.Y = Math.Min(0, physics.Acceleration.Y);
            }
            if (collidesBottom) {
                //flip Y axis
                if (physics.Velocity.Y < 0) physics.Velocity.Y = Math.Max(-physics.Velocity.Y * Elasticity, minimumCollisionVelocity);
                physics.Acceleration.Y = Math.Max(0, physics.Acceleration.Y);
            }
            if (collidesRight) {
                //flip X axis
                if(physics.Velocity.X < 0) physics.Velocity.X = Math.Max(-physics.Velocity.X * Elasticity, minimumCollisionVelocity);
                physics.Acceleration.X = Math.Max(0, physics.Acceleration.X);
            }
            if (collidesLeft) {
                //flip X axis
                if (physics.Velocity.X > 0) physics.Velocity.X = Math.Min(-physics.Velocity.X * Elasticity,-minimumCollisionVelocity);
                physics.Acceleration.X = Math.Min(0, physics.Acceleration.X);
            }
        }

        //see: https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection
        private bool CollidesWith(Ball ball, Rect rect) {
            var rect_width = rect.x2 - rect.x1;
            var rect_height = rect.y2 - rect.y1;

            //distance x/y van bal en midden van rechthoek
            var circleDistance_x = Math.Abs(ball.X - (rect.x1 + rect_width / 2 ));
            var circleDistance_y = Math.Abs(ball.Y - (rect.y1 + rect_height / 2 ));

            //als afstand midden rechthoek -> midden bal > straal bal + helft breedte/hoogte rechthoek -> kan niet overlappen
            if (circleDistance_x > (rect_width / 2 + ball.Size)) { return false; }
            if (circleDistance_y > (rect_height / 2 + ball.Size)) { return false; }

            //anders wel
            if (circleDistance_x <= (rect_width / 2)) { return true; }
            if (circleDistance_y <= (rect_height / 2)) { return true; }

            //zet perspectief naar hoekpunten (allemaal evenwaardig wegens bekijken centrum)
            //pythagoras (^2)
            var cornerDistance_sq = Math.Pow((circleDistance_x - rect_width / 2),2) +
                                 Math.Pow((circleDistance_y - rect_height / 2), 2);

            //return afstand naar hoekpunt < bal.straal (^ 2; geen wortel nodig aangezien a^2+b^2=c^2)
            return (cornerDistance_sq <= (ball.Size *ball.Size));
        }
    }
}
