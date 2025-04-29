class Container:
    def __init__(self, x = 0, y = 0, length = 0, height = 0, width = 0):
        self.x = x
        self.y = y
        self.length = length
        self.height = height
        self.width = width
    
    def get_coordinates(self):
        return self.x, self.y

    def get_dimensions(self):
        return self.height, self.length, self.width

if __name__ == "__main__":
    start_x = -4
    start_y = -10

    crane = Container(x=start_x, y=start_y, height=12, length=13, width=32)

    print("Current coordinates:", crane.get_coordinates())
    print("Crane dimensions:", crane.get_dimensions())

