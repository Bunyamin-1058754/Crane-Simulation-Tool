# hier zijn de componenten van de kraan onderverdeeld in:


# gantry, hierbij horen de portaal brug ligger en de klap ligger
class GantryStructuur:
    def __init__(self, portal, bridge_layer, folding_bed):
        self.portal = portal
        self.bridge_layer = bridge_layer
        self.folding_bed = folding_bed


class FoldingBed:
    def __init__(self, bridge_layer):
        self.bridge_layer = bridge_layer
        self.is_folded = True


class Portaal:
    def __init__(self, length, width):
        self.length = length
        self.width = width
        self.x = 0
        self.y = 0


class Brugligger:
    def __init__(self, portal):
        self.portal = portal
        self.x = 0
        self.y = 0


# trolley en de spreaderbeam
class Trolley:
    def __init__(self, bridge_layer, spreader_beam_length):
        self.bridge_layer = bridge_layer
        self.spreader_beam_length = spreader_beam_length
        self.x = 0


class SpreaderBeam:
    def __init__(self, hoist):
        self.hoist = hoist


# hoist system
class HoistSystem:
    def __init__(
        self,
        trolley,
        num_cables,
        cable_length,
    ):
        self.trolley = trolley
        self.is_container_attached = False
        self.container_height = 0
        self.cables = [Cable(length=cable_length) for _ in range(num_cables)]

    def attach_container(self):
        self.is_container_attached = True
        print("Container attached to hoist system")

    def detach_container(self):
        self.is_container_attached = False
        print("Container detached from hoist system")

    def lift_container(self, amount):
        if self.is_container_attached:
            for cable in self.cables:
                cable.decrease_length(amount)
            print(
                f"Container lifted. Cable lengths: {[cable.length for cable in self.cables]}"
            )
        else:
            print("No container attached to lift")

    def lower_container(self, amount):
        if self.is_container_attached:
            for cable in self.cables:
                cable.increase_length(amount)
            print(
                f"Container lowered. Cable lengths: {[cable.length for cable in self.cables]}"
            )
        else:
            print("No container attached or already at the ground level")


class Cable:
    def __init__(self, length):
        self.length = length

    def decrease_length(self, amount):
        if self.length - amount >= 0:
            self.length -= amount
            print(f"Cable length decreased to {self.length}")
        else:
            print("Cannot decrease cable length further")

    def increase_length(self, amount):
        self.length += amount
        print(f"Cable length increased to {self.length}")


# STS CRANE
class STSCrane:
    def __init__(self):
        self.portal = None
        self.bridge_layer = None
        self.trolley = None
        self.hoist = None
        self.spreader_beam = None
        self.folding_bed = None
        self.gantry = None

    def create_components(self):
        self.portal = Portaal(length=50, width=50)
        self.bridge_layer = Brugligger(portal=self.portal)
        self.trolley = Trolley(bridge_layer=self.bridge_layer, spreader_beam_length=10)
        self.hoist = HoistSystem(trolley=self.trolley)
        self.spreader_beam = SpreaderBeam(hoist=self.hoist)
        self.folding_bed = FoldingBed(bridge_layer=self.bridge_layer)
        self.gantry = GantryStructuur(
            portal=self.portal,
            bridge_layer=self.bridge_layer,
            folding_bed=self.folding_bed,
        )

    def get_coordinates(self):
        return {
            "Portal": (self.portal.x, self.portal.y),
            "BridgeLayer": (self.bridge_layer.x, self.bridge_layer.y),
            "Trolley": (self.trolley.x, self.trolley.bridge_layer.y),
            "FoldingBed": (
                self.folding_bed.bridge_layer.x,
                self.folding_bed.bridge_layer.y,
            ),
            "Gantry": (self.gantry.portal.x, self.gantry.portal.y),
        }


HoistSystem(3, 324, 43)

if __name__ == "__main__":
    crane = STSCrane()
    crane.create_components()

    # Get and print the coordinates of the components
    coordinates = crane.get_coordinates()
    for component, coord in coordinates.items():
        print(f"{component} coordinates: {coord}")
