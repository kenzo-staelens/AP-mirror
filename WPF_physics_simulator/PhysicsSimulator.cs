using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;
using Components;
using System.ComponentModel;

namespace WPF_physics_simulator {
    public class PhysicsSimulator {
        public Rect[] Environment;
        public Ball PhysicsEntity;
        private double Elasticity;
        private double g; // zwaartekracht cte
        private double PhysicsMass;
        private double max_velocity = 0.5;
        private double max_acc = 0.05;
        private int CellCountWidth;
        private int CellCountHeight;
        private int CellSize;


        public PhysicsSimulator(Rect[] environment, Ball ball, int cellCountWidth, int cellCountHeight, int cellsize) {
            this.Environment = environment; //assumption walls are immovable
            this.PhysicsEntity = ball;
            this.Elasticity = 0;//in interval [0,1]
            this.g = 9.81;//N
            this.PhysicsMass = 5e8;
            this.CellCountWidth = cellCountWidth;//optimization data
            this.CellCountHeight = cellCountHeight;
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
            
            Rect? collidingRect = null;
            for(int i=0;i< this.Environment.Length;i++) {
                if (!validIndexes.Contains(Environment[i].source_cell)){
                    Environment[i].mark(false);
                    continue;//wall too far to check
                }
                Environment[i].mark(true);
                if (CollidesWith(PhysicsEntity, Environment[i])){
                    collidingRect = Environment[i];//ball collides with wall
                    break;
                }
            }

            Vector[]? CollisionVectors = CalculateCollisionVector(collidingRect);
            if (CollisionVectors != null) {
                //physicsComponent.Velocity = CollisionVectors[0];
                //physicsComponent.Acceleration = CollisionVectors[1]; //resets acceleration in collision direction
            }

            if (Math.Abs(physicsComponent.Velocity.X) > max_velocity) {
                physicsComponent.Velocity.X = max_velocity*Math.Sign(physicsComponent.Velocity.X);
            }
            if (Math.Abs(physicsComponent.Velocity.Y) > max_velocity) {
                physicsComponent.Velocity.Y = max_velocity * Math.Sign(physicsComponent.Velocity.Y);
            }

            if (Math.Abs(physicsComponent.Acceleration.X) > max_acc) {
                physicsComponent.Acceleration.X = max_acc * Math.Sign(physicsComponent.Acceleration.X);
            }
            if (Math.Abs(physicsComponent.Acceleration.Y) > max_acc) {
                physicsComponent.Acceleration.Y = max_acc*Math.Sign(physicsComponent.Acceleration.Y);
            }

            PhysicsEntity.X += physicsComponent.Velocity.X * dt_millis;// + physicsComponent.Acceleration.X * dt_millis * dt_millis / 2;
            PhysicsEntity.Y += physicsComponent.Velocity.Y * dt_millis;// + physicsComponent.Acceleration.X * dt_millis * dt_millis / 2;
            physicsComponent.Velocity.X += physicsComponent.Acceleration.X * dt_millis;
            physicsComponent.Velocity.Y += physicsComponent.Acceleration.Y * dt_millis;
            physicsComponent.Acceleration.X += physicsComponent.Force.X * dt_millis / PhysicsMass;
            physicsComponent.Acceleration.Y += physicsComponent.Force.Y * dt_millis / PhysicsMass;
            return physicsComponent;//for printing statistics
        }

        private Vector[]? CalculateCollisionVector(Rect? collidingRect) {
            if (collidingRect == null) return null;
            return null;
            //todo calculate collision
            //maakt gebruik van velocity vector

            //if ladder -> collision = verticaal of horizontaal
            // -> flip velocity x/y depending on horizontal/vertical
            //multiply by elasticity on reflecting vector component
            //return new Vector(0, 0);
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
