M = {}

M.name = 'test.02'
M.myaction = -1

function M: startfunction(gamestat)    --游戏最开始(初始化，等待玩家响应)  
    print(M['name'] , "初始化成功！")
    return M['name']
end

function M: round_start(gamestat)    --每【场】开始(仅返回每个玩家开局时情况)
    print( M['name'] , "开局！") 
    return
end

function M: action(gamestat)    --每【轮】调用动作(返回详细信息，等待玩家操作)
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    ranNum = math.random(100)
    if (ranNum <= 50) then
        M.myaction = 1
        return M.myaction
    elseif (ranNum <= 90) then
        M.myaction = 2
        return M.myaction
    elseif (ranNum <= 98) then
        M.myaction = 3
        return M.myaction
    end
    M.myaction = 4
    return M.myaction
end

function M: finalCards(gamestat)    --每【轮】调用动作(返回详细信息，等待玩家操作)
    list = {}
    list[1] = gamestat.CardsInHands[0]
    list[2] = gamestat.CardsInHands[1]
    list[3] = gamestat.CommunityCards[0]
    list[4] = gamestat.CommunityCards[1]
    list[5] = gamestat.CommunityCards[2]
    return list
end