M = {
    name = 'what003',
    myaction = -1;
}

function M: startfunction(gamerule)    --游戏最开始(初始化，等待玩家响应)  
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
    math.randomseed(tostring(os.time()):reverse():sub(1, 7))
    first = math.random(10000) % 3
    if (first == 0) then
        second = math.random(10000) % 7
        if (second == 0) then second = second+1 end
    else
        second = 2*first
    end
    list = {}
    no = 0
    for i=-1,6,1 do
        if (i == first)  then goto continue end
        if (i == second) then goto continue end
        list[no] = i
        no = no + 1
        ::continue::
    end
    return list

end