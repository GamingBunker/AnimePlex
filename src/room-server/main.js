const Boom          = require("@hapi/boom")
const HAPI          = require("@hapi/hapi")
const WebSocket     = require('ws');
const uuid          = require('uuid');
const schedule      = require('node-schedule');

require('dotenv').config();
;(async () => {
    /*  create new HAPI service  */
    //const server = new HAPI.Server({ address: "localhost", port: 12345 })
    //socket
    const wss = new WebSocket.Server({
        port: process.env.PORT,
        host: process.env.HOST,
        path: process.env.PATH_URL
    });
    console.log("Settings:")
    console.log("Host: "+wss.options.host);
    console.log("Port: "+wss.options.port);
    console.log("Path: "+wss.options.path);

    //variables global
    const clients = new Map();

    //rooms
    const rooms = []
    /*
    {
        id_room:'balbalblabla',
        clients:[
            {
                id:'81298dj19jd1',
                nickname:'pippo'
            },
            {
                ...
            }
        ]
    }
    */
    const MAX_NUMBER_CLIETNS_ROOM = process.env.MAX_NUM
    wss.on('connection', (ws) =>{
        const id = uuid.v4();
        clients.set(ws, id)
        //console.log(clients)

        ws.on('message', (message)=>{
            var json = JSON.parse(message)
            
            var action = json.action
            var data = json.data

            if(action === 'pong'){
                //nothing
            }else if(action === 'create'){
                //registration
                var idRoom = createRoom(ws, data)

                //send confirm registration
                ws.send(JSON.stringify({action: 'registration', id:clients.get(ws), nickname: data.nickname}))
                alertNewMember(idRoom)

            }else if(action === 'update'){
                //console.log('un messagio da inoltrare')
                //console.log(data)
                
                const wsContact = [...clients].find(([key, val]) => val == data.data.idContact)[0]
                const data = data.data
                wsContact.send(JSON.stringify({action:'receive', data}))
            }else if(action === 'join'){
                var room = joinRoom(ws, data);

                console.log(room);

                ws.send(JSON.stringify({action: 'loadVideo', id:clients.get(ws), nickname: data.nickname, episode: room.episode}))
                alertNewMember(room.id_room)
            }else if(action === 'updatePause'){
                rooms.map(room => {
                    if(room.id_room == data.idRoom){
                        room.pause = data.pause
                    }
                })

                alertNewMember(data.idRoom);
            }else if(action == 'updateTime'){
                rooms.map(room => {
                    if(room.id_room == data.idRoom){
                        room.t = data.time
                    }
                })

                alertNewMember(data.idRoom);
            }
        })

        ws.on("close", () => {
            var id = clients.get(ws)

            //find id room
            var id_room_client = getIdRoomByIdClient(id)
            removeClientIntoRoom(id_room_client, id)

            //remove client
            clients.delete(ws);
        });
    })

    //methods
    function joinRoom(ws, data){
        const client = {
            id: clients.get(ws),
            nickname: data.nickname
        }

        rooms.map(room => {
            if(room.id_room === data.idRoom){
                console.log('joined');
                room.clients.push(client)
            }
        })
        return rooms.filter(room => room.id_room === data.idRoom)[0];
    }
    function getIdRoomByIdClient(id){
        var id_room_client = null
        for(var i=0; i<rooms.length; i++)
        {
            const find = rooms[i].clients.find(element => element.id === id) || null
            if(find != null)
            {
                id_room_client=rooms[i].id_room
                break
            }
        }
        return id_room_client;
    }

    function removeClientIntoRoom(idRoom, id)
    {
        //if this user not exist in rooms
        if(idRoom != undefined)
        {

            //delete client
            rooms.forEach((room )=>{
                room.clients.find((client, index)=>{
                    if(client != undefined && client.id === id)
                    {
                        room.clients.splice(index, 1);
                    }
                })
            })

            //get class room            
            const room = rooms.find(element => element.id_room === idRoom)
            room.clients.forEach(item =>{
                const ws = [...clients].find(([key, val]) => val == item.id)[0]
                const find = room.clients.find(element => element.id !== id) || null

                //send clients that one client is closed
                if(find != null)
                    ws.send(JSON.stringify({action: 'UpdateRoom', room:room}))
            })

            //delete room if aren't contacts
            for(var i=0; i<rooms.length; i++)
            {
                if(rooms[i].clients.length == 0){
                    rooms.slice(i, 1)
                }
            }
        }
    }

    function createRoom(ws, data){
        const client = {
            id: clients.get(ws),
            nickname: data.nickname
        }

        id_room = uuid.v4()
        rooms.push({
            id_room:id_room,
            pause:true,
            t:0,
            episode:data.episode,
            clients:[client]
        })

        return id_room;
    }
    
    function alertNewMember(id_room_client){
        //sent other contacts for room
        const room = rooms.find(element => element.id_room === id_room_client)
        room.clients.forEach(item =>{
            const ws = [...clients].find(([key, val]) => val == item.id)[0]
            const find = room.clients.find(element => element.id === item.id) || null
            if(find != null)
                ws.send(JSON.stringify({action: 'UpdateRoom', room}))
        })
    }

    //schedule
    schedule.scheduleJob('*/5 * * * *', function(){
        clients.forEach(client_id => {
            const ws = [...clients].find(([key, val]) => val == client_id)[0]
            ws.send(JSON.stringify({action:"ping"}))
        });
    });
})().catch((err) => {
    console.log(`ERROR: ${err}`)
})