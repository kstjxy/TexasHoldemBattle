M = {}

M.name = 'test.02'
M.myaction = -1

function M: startfunction(gamestat)    --��Ϸ�ʼ(��ʼ�����ȴ������Ӧ)  
    print(M['name'] , "��ʼ���ɹ���")
    return M['name']
end

function M: round_start(gamestat)    --ÿ��������ʼ(������ÿ����ҿ���ʱ���)
    print( M['name'] , "���֣�") 
    return
end

function M: action(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
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

function M: finalCards(gamestat)    --ÿ���֡����ö���(������ϸ��Ϣ���ȴ���Ҳ���)
    list = {}
    list[1] = gamestat.CardsInHands[0]
    list[2] = gamestat.CardsInHands[1]
    list[3] = gamestat.CommunityCards[0]
    list[4] = gamestat.CommunityCards[1]
    list[5] = gamestat.CommunityCards[2]
    return list
end