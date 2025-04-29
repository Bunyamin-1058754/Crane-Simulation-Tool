import pygame
import os
from container import Container
from mqttConnect import *
import time
from craneComponents import Cable, HoistSystem


folder_path = os.path.dirname(__file__)
folder_path += "/PythonImages"

connect_mqtt()

pygame.init()

SCREEN_WIDTH = 600
SCREEN_HEIGHT = 710
SIM_SPEED = 160

grabbed_container = False

screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))

spreader = pygame.Rect((255, 0, 50, 10))

pygame.font.init()

container_instance = Container(45, 636, 45, 20, 47)
container = pygame.Rect(
    container_instance.x,
    container_instance.y,
    container_instance.width,
    container_instance.height,
)
cable_1 = pygame.Rect((278, 355, 5, 0))
cable = Cable(length=0)

frame_number = 1

clock = pygame.time.Clock()


def drawCrane():
    # Drawing the side view of the crane
    # Legs
    pygame.draw.line(screen, (255, 255, 0), (475, 290), (475, 650), 10)
    pygame.draw.line(screen, (255, 255, 0), (325, 200), (325, 650), 10)

    pygame.draw.line(screen, (255, 255, 0), (325, 200), (575, 350), 10)
    pygame.draw.line(screen, (255, 255, 0), (325, 200), (30, 350), 10)
    pygame.draw.line(screen, (255, 255, 0), (325, 350), (475, 600), 10)
    # Boom
    pygame.draw.line(screen, (255, 255, 0), (30, 350), (575, 350), 10)


def saveImages():
    global frame_number
    pygame.image.save(screen, f"{folder_path}/image_{frame_number - 1}.png")


def publishToServer():
    global frame_number
    saveImages()
    client.publish(topic="simulation/crane/image", payload=f"{frame_number - 1}", qos=0)
    frame_number += 1


def is_spreader_near_container():
    distance_threshold = 20
    return (
        abs(spreader.centerx - container.centerx) < distance_threshold
        and abs(spreader.centery - container.centery) < distance_threshold
    )


def toggleSpreaderGrabContainer():
    global grabbed_container
    if is_spreader_near_container():
        grabbed_container = not grabbed_container
        if grabbed_container == True:
            print("Grabbed container")
            time.sleep(0.5)
        elif grabbed_container == False:
            print("Let go of container")
            time.sleep(0.5)
    else:
        print("Spreader is not close enough to the container to grab it")
        time.sleep(0.5)


def sendCoordsToServer():
    xCoord = spreader[0]
    yCoord = spreader[1]
    coords = f"{xCoord}, {710 - yCoord}"
    client.publish(topic="simulation/crane/coordinates", payload=coords, qos=0)
    print(coords)


def sendMovementToServer(direction):
    client.publish(topic="simulation/crane/movement", payload=direction, qos=0)


def sendContainerCoordsToServer():
    if grabbed_container:
        xCoord = container[0]
        yCoord = container[1]
        coords = f"{xCoord}, {710 - yCoord}"
        client.publish(topic="simulation/container/coordinates", payload=coords, qos=0)
        print(coords)


paused = False
font = pygame.font.SysFont(None, 36)
run = True
while run:
    screen.fill((0, 0, 0))
    drawCrane()

    # Spreader
    pygame.draw.rect(screen, (255, 23, 0), spreader)
    # Cable
    pygame.draw.rect(screen, (255, 255, 0), cable_1)
    # Container 
    pygame.draw.rect(screen, (0, 0, 255), container)
    
    if paused:
        paused_text = font.render("Simulation Paused", True, (255, 0, 0))
        screen.blit(paused_text, (SCREEN_WIDTH // 2 - 120, SCREEN_HEIGHT // 5 - 18))
    else:
        saveImages()
        

        # Controls
        key = pygame.key.get_pressed()
        if key[pygame.K_UP] == True and spreader[1] != 355:
            spreader.move_ip(0, -1)
            Cable.decrease_length(self=cable, amount=1)
            cable_1[3] = cable.length
            sendCoordsToServer()
            sendMovementToServer("Up")
            sendContainerCoordsToServer()
            publishToServer()
        elif key[pygame.K_DOWN] == True and spreader[1] != 636:
            spreader.move_ip(0, +1)
            if not spreader.colliderect(container):
                Cable.increase_length(self=cable, amount=1)
                cable_1[3] = cable.length
            elif grabbed_container:
                Cable.increase_length(self=cable, amount=1)
                cable_1[3] = cable.length
            sendCoordsToServer()
            sendMovementToServer("Down")
            sendContainerCoordsToServer()
            publishToServer()
        elif key[pygame.K_LEFT] == True and spreader[0] != 45:
            spreader.move_ip(-1, 0)
            cable_1.move_ip(-1, 0)
            sendCoordsToServer()
            sendMovementToServer("Forward")
            sendContainerCoordsToServer()
            publishToServer()
        elif key[pygame.K_RIGHT] == True and spreader[0] != 375:
            spreader.move_ip(+1, 0)
            cable_1.move_ip(+1, 0)
            sendCoordsToServer()
            sendMovementToServer("Backward")
            sendContainerCoordsToServer()
            publishToServer()
        elif key[pygame.K_RETURN] == True:
            if container.y < 636:
                font = pygame.font.Font("freesansbold.ttf", 32)
                text = font.render("Cannot release at this height", True, (255, 0, 0))
                textRect = text.get_rect()
                textRect.center = (SCREEN_WIDTH // 2, SCREEN_HEIGHT // 5)
                screen.blit(text, textRect)
            else:
                toggleSpreaderGrabContainer()
                sendMovementToServer("Grab")

        if grabbed_container:
            container.x = spreader.x
            container.y = spreader.y + 10

        # Boundaries for the spreader
        if spreader.centerx >= 400:
            spreader.centerx = 400
        if spreader.centerx <= 50:
            spreader.centerx = 50
        if spreader.centery >= 641:
            spreader.centery = 641
        if spreader.centery <= 360:
            spreader.centery = 360

        if spreader.colliderect(container):
            spreader.y = container.y - spreader.height

        containerX = container[0]
        containerY = container[1] - 10
        if spreader[0] == containerX and spreader[1] >= containerY:
            spreader[1] = containerY
                

        if cable_1.centerx >= 400:
            cable_1.centerx = 400
        if cable_1.centerx <= 50:
            cable_1.centerx = 50
        if cable_1.centery >= 641:
            cable_1.centery = 641
        if cable_1.centery <= 340:
            cable_1.centery = 340

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            disconnect_mqtt()
            for i in range(frame_number):
                os.remove(f"{folder_path}/image_{i}.png")
            run = False
        elif event.type == pygame.KEYDOWN:
            if event.key == pygame.K_p:
                client.publish(topic="simulation/paused", payload="pause")
                paused = not paused

    pygame.display.update()
    clock.tick(SIM_SPEED)

pygame.quit()